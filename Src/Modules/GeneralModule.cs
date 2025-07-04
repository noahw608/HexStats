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
    
    public GeneralModule(ILogger<GeneralModule> logger, IUserRepository userRepository, ILeagueService leagueService)
    {  
        _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository), "User repository cannot be null.");
        _leagueService = leagueService ?? throw new ArgumentNullException(nameof(leagueService), "League service cannot be null.");
        _logger = logger ?? throw new ArgumentNullException(nameof(logger), "Logger cannot be null.");
    }
    
    [SlashCommand("ping", "Check if the bot is responsive. Response with \"Pong!\"")]
    public async Task PingCommandAsync()
    {
        await RespondAsync("Pong!", ephemeral: true);
    }

    [SlashCommand("register", "Register an account to be tracked by HexStats.")]
    public async Task RegisterCommandAsync(
        [Summary("Region", "The Region of the game account to register (e.g., NA, EUW, KR).")] GameRegion gameRegion,
        [Summary("Username", "The username of the game account to register.")] string username,
        [Summary("Tagline", "The tag of the game account to register (e.g., #1234).")] string tagline
    )
    {

        AccountRegion accountRegion = LeagueService.MapGameRegionToAccountRegion(gameRegion);
        
        
        // Check if user has associated Riot account
        AccountDto accountDto = await _leagueService.GetRiotAccountByLeagueNameAsync(username, tagline, accountRegion);
        Log.Information("Got accountDto with values: {username}:{tagline}, and puuid: {puuid}", accountDto.gameName, accountDto.tagLine, accountDto.puuid);

        SummonerDto summonerDto = await _leagueService.GetSummonerByPuuidAsync(accountDto.puuid, gameRegion);
        Log.Information("Summoner Info retrieved for PUUID {puuid}: ProfileIconId: {profileIconId}, SummonerLevel: {summonerLevel}",
            summonerDto.puuid, summonerDto.profileIconId, summonerDto.summonerLevel);
        
        
        await _userRepository.AddUserAsync(new User
        {
            DiscordUsername = Context.User.Username,
            LeagueUsername = username,
            LeagueTagline = tagline,
            LeagueGameRegion = gameRegion
        });
        
        var embed = new EmbedBuilder()
            .WithTitle("Account Registered!")
            .WithDescription(
                $"Discord User: {Context.User.Username}#{Context.User.Discriminator} has been registered as Summoner: **{username}**:**{tagline}** in Region **{gameRegion}** and will now have their games tracked.")
            .WithColor(Color.Green)
            .Build();
        
        await RespondAsync(embed: embed, ephemeral: true);
    }
}