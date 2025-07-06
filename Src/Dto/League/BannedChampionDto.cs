namespace HexStats.Dto.League;

/// <summary>
/// Represents information about a banned champion in a League of Legends match
/// </summary>
public class BannedChampionDto
{
    /// <summary>
    /// The turn during which the champion was banned
    /// </summary>
    public int pickTurn { get; set; }

    /// <summary>
    /// The ID of the banned champion
    /// </summary>
    public long championId { get; set; }

    /// <summary>
    /// The ID of the team that banned the champion
    /// </summary>
    public long teamId { get; set; }
}
