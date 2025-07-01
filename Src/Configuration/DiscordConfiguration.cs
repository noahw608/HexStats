namespace HexStats.Configuration;

public class DiscordConfiguration
{
    public string Token { get; set; } = string.Empty;
    public string Intents { get; set; } = string.Empty;
    public List<Guild> Guilds { get; set; } = new List<Guild>();
}

public class Guild
{
    public ulong GuildId { get; set; }
    public ulong ChannelId { get; set; }
}