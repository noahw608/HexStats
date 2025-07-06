using System.ComponentModel;
using Discord;
using Discord.Interactions;
using HexStats.Dto.League;
using HexStats.Enums;
using HexStats.Models;
using HexStats.Repositories;
using HexStats.Services;
using HexStats.Services.Interfaces;
using Microsoft.Extensions.Logging;
using Serilog;

namespace HexStats.Modules;

public class GeneralModule: InteractionModuleBase<SocketInteractionContext>
{

    private readonly ILeagueService _leagueService;
    private readonly ILogger<GeneralModule> _logger;
    private readonly IUserRepository _userRepository;

    private static readonly Color StandardColor = Color.Parse("#2d89ca");
    
    public GeneralModule(ILogger<GeneralModule> logger, IUserRepository userRepository, ILeagueService leagueService)
    {  
        _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository), "User repository cannot be null.");
        _leagueService = leagueService ?? throw new ArgumentNullException(nameof(leagueService), "League service cannot be null.");
        _logger = logger ?? throw new ArgumentNullException(nameof(logger), "Logger cannot be null.");
    }
    
    // PING COMMAND
    [SlashCommand("ping", "Check if the bot is responsive. Response with \"Pong!\"")]
    public async Task PingCommandAsync()
    {
        await RespondAsync("Pong!", ephemeral: true);
    }

    // CLEAR DATABASE COMMAND
    [SlashCommand("clear-database", "Clear the entire database. This will delete all registered users and their data.")]
    [RequireOwner]
    public async Task ClearDatabaseCommandAsync([Summary("Confirmation", "Please confirm you would like to clear the database. This operation is PERMANENT")] bool confirmation)
    {
        if (confirmation)
        {
            List<User> users = await _userRepository.GetAllUsersAsync();
            foreach (User user in users)
            {
                await _userRepository.DeleteUserAsync(user.Id);
            }
            
            await RespondAsync(embed: CreateStandardEmbed(
                title: "**Database Cleared**",
                description: "All registered users and their data have been deleted from the database."
                ), ephemeral: true);
            return;
        }
        
        await RespondAsync(embed: CreateErrorEmbed(
            title: "**Database Clear Failed**",
            description: "You must confirm the operation by setting the confirmation parameter to true."
            ), ephemeral: true);
        _logger.LogWarning("Database clear operation was requested but not confirmed.");
    }

    // REGISTER COMMAND
    [SlashCommand("register", "Register an account to be tracked by HexStats.")]
    public async Task RegisterCommandAsync(
        [Summary("Region", "The Region of the game account to register (e.g., NA, EUW, KR).")] GameRegion gameRegion,
        [Summary("Username", "The username of the game account to register.")] string username,
        [Summary("Tagline", "The tag of the game account to register (e.g., #1234).")] string tagline
    )
    {
        
        if (await _userRepository.UserExistsByDiscordUsernameAsync(Context.User.Username))
        {
            Log.Warning("User {username} already registered in database, cannot register again.", Context.User.Username);
            await RespondAsync(embed: CreateErrorEmbed(
                title: "**Registration Failed**",
                description: $"You have already registered an account with HexStats. Please use `/unregister` to remove your current registration before registering a new one."
                ), ephemeral: true);
            return;
        }

        if (await _userRepository.UserExistsByLeagueUsernameAsync(username, tagline, gameRegion))
        {
            Log.Warning("Requested summoner already registered and exists in database: {username}#{tagline} in {gameRegion}", username, tagline, gameRegion);
            User user = await _userRepository.GetUserByLeagueUsernameAsync(username, tagline, gameRegion);
            
            await RespondAsync(embed: CreateErrorEmbed(
                title: "**Registration Failed**",
                description: $"The summoner **{username}#{tagline}** has already been linked to discord user **{user.DiscordUsername}** in region **{gameRegion}**."
                ), ephemeral: true);
            return;
        }
        
        AccountRegion accountRegion = LeagueService.MapGameRegionToAccountRegion(gameRegion);
        AccountDto accountDto;

        try
        {
            accountDto = await _leagueService.GetRiotAccountByLeagueNameAsync(username, tagline, accountRegion);
            Log.Information("Got accountDto with values: {username}:{tagline}, and puuid: {puuid}", accountDto.gameName,
                accountDto.tagLine, accountDto.puuid);
        }
        catch (Exception ex)
        {
            Log.Warning(ex, "Failed to retrieve account information for {username}#{tagline} in {gameRegion}", username, tagline, gameRegion);
            
            await RespondAsync(embed: CreateErrorEmbed(
                title: "**Registration Failed**",
                description: $"Failed to retrieve account information for **{username}#{tagline}** in **{gameRegion}**. Please check the username and tagline."
                ), ephemeral: true);
            return;
        }
        
        SummonerDto summonerDto = await _leagueService.GetSummonerByPuuidAsync(accountDto.puuid, gameRegion);
        Log.Information("Summoner Info retrieved for PUUID {puuid}: ProfileIconId: {profileIconId}, SummonerLevel: {summonerLevel}",
            summonerDto.puuid, summonerDto.profileIconId, summonerDto.summonerLevel);
        
        await _userRepository.AddUserAsync(new User
        {
            DiscordUsername = Context.User.Username,
            LeagueUsername = username,
            LeagueTagline = tagline,
            LeagueGameRegion = gameRegion,
            Puuid = summonerDto.puuid,
            SummonerIconId = summonerDto.profileIconId,
            SummonerLevel = summonerDto.summonerLevel
        });
        
        await RespondAsync(embed: CreateRegistrationEmbed(
            username, tagline, Context.User.Username, summonerDto.profileIconId, _leagueService.Configuration.DataDragonVersion),
            ephemeral: false);
    }
    
    // UNREGISTER COMMAND
    [SlashCommand("unregister", "Unregister your account from HexStats.")]
    public async Task UnregisterCommandAsync()
    {

        User? user;

        Log.Debug("Unregister called without specific parameters, getting user by Discord username: {username}", Context.User.Username);
        user = await _userRepository.GetUserByDiscordUsernameAsync(Context.User.Username);
        if (user == null)
        {
            Log.Warning("Unregister command called by {username} but no user found in database.", Context.User.Username);
            await RespondAsync(embed: CreateErrorEmbed(
                title: "**Unregistration Failed**",
                description: "Your discord account has not been registered to any League account with HexStats. Please register first using `/register`."
                ), ephemeral: true);
            return;
        }
        
        await _userRepository.DeleteUserAsync(user.Id);
        
        await RespondAsync(embed: CreateStandardEmbed(
            title: "**Unregistration Successful**",
            description: $"{Context.User.Username}'s account **{user.LeagueUsername}#{user.LeagueTagline}** has been unregistered from HexStats."
        ), ephemeral: false);
    }
    
    // LIST USERS COMMAND
    [SlashCommand("list-users", "List all registered users in HexStats.")]
    public async Task ListUsersCommandAsync()
    {
        List<User> users = await _userRepository.GetAllUsersAsync();
        if (users.Count == 0)
        {
            await RespondAsync(embed: CreateErrorEmbed(
                title: "**No Users Registered**",
                description: "There are currently no registered users in HexStats."
            ), ephemeral: true);
            return;
        }
        
        await RespondAsync(embed: CreateListEmbed(users), ephemeral: false);
        
    }
    
    public static Embed CreateStandardEmbed(string title, string description)
    {
        return new EmbedBuilder()
            .WithTitle(title)
            .WithColor(StandardColor)
            .WithDescription(description)
            .Build();
    }
    
    public static Embed CreateErrorEmbed(string title, string description)
    {
        return new EmbedBuilder()
            .WithTitle(title)
            .WithColor(255, 0, 0)
            .WithDescription(description)
            .Build();
    }
    
    public static Embed CreateRegistrationEmbed(string leagueUsername, string leagueTagLine, string discordUsername, int iconId, string dataDragonVersion)
    {
        return new EmbedBuilder() // todo: Move to separate function
            .WithTitle($"**Summoner Registered : {leagueUsername}#{leagueTagLine}**")
            .WithThumbnailUrl(GetLeagueIconUrl(iconId, dataDragonVersion))
            .WithColor(StandardColor)
            .WithDescription(
                $"**{discordUsername}** has linked summoner **{leagueUsername}#{leagueTagLine}** in HexStats.")
            .Build();
    }
    
    public static Embed CreateListEmbed(List<User> users)
    {
        EmbedFieldBuilder[] fields = new EmbedFieldBuilder[users.Count];
        for (int i = 0; i < users.Count; i++)
        {
            User user = users[i];
            fields[i] = new EmbedFieldBuilder()
                .WithValue($"Summoner Username: {user.LeagueUsername}#{user.LeagueTagline} ({user.LeagueGameRegion})")
                .WithName($"{user.DiscordUsername}")
                .WithIsInline(false);
        }
        
        return new EmbedBuilder()
            .WithTitle("Registered Users")
            .WithColor(StandardColor)
            .WithFields(fields)
            .Build();
    }
    
    public static string GetLeagueIconUrl(int iconId, string dataDragonVersion)
    {
        return $"https://ddragon.leagueoflegends.com/cdn/{dataDragonVersion}/img/profileicon/{iconId}.png";
    }
}