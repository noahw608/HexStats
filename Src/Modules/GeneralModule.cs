using System.ComponentModel;
using Discord;
using Discord.Interactions;
using HexStats.Enums;

namespace HexStats.Modules;

public class GeneralModule: InteractionModuleBase<SocketInteractionContext>
{
    [SlashCommand("ping", "Check if the bot is responsive. Response with \"Pong!\"")]
    public async Task PingCommandAsync()
    {
        await RespondAsync("Pong!", ephemeral: true);
    }

    [SlashCommand("register", "Register an account to be tracked by HexStats.")]
    public async Task RegisterCommandAsync(
        [Summary("Game Region", "The Region of the game account to register (e.g., NA, EUW, KR).")] GameRegion region,
        [Summary("Username", "The username of the game account to register.")] string username,
        [Summary("Tagline", "The tag of the game account to register (e.g., #1234).")] string tagline
    )
    {
        var embed = new EmbedBuilder()
            .WithTitle("Account Registered!")
            .WithDescription(
                $"Discord User: {Context.User.Username}#{Context.User.Discriminator} has been registered as Summoner: **{username}**:**{tagline}** in Region **{region}** and will now have their games tracked.")
            .WithColor(Color.Green)
            .Build();
        
        await RespondAsync(embed: embed, ephemeral: true);
    }
}