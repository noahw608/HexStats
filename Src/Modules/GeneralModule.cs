using Discord.Interactions;

namespace HexStats.Modules;

public class GeneralModule: InteractionModuleBase<SocketInteractionContext>
{
    [SlashCommand("ping", "Check if the bot is responsive. Response with \"Pong!\"")]
    public async Task PingCommandAsync()
    {
        await RespondAsync("Pong!", ephemeral: true);
    }
}