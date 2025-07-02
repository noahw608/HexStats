namespace HexStats.Configuration;

public class DiscordConfiguration
{
    public string Token { get; set; } = string.Empty;
    public string Intents { get; set; } = string.Empty;
    public ulong GuildId { get; set; }
    public ulong ChannelId { get; set; }
}
