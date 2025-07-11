using HexStats.Enums;

namespace HexStats.Models;

public class User
{
    public int Id { get; set; }
    public string DiscordUsername { get; set; } = string.Empty;
    public string LeagueUsername { get; set; } = string.Empty;
    public string LeagueTagline { get; set; } = string.Empty;
    public GameRegion LeagueGameRegion { get; set; }
    public string Puuid { get; set; } = string.Empty;
    public int SummonerIconId { get; set; }
    public int SummonerLevel { get; set; }
}