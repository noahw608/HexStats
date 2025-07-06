namespace HexStats.Dto.League;

/// <summary>
/// Represents the current game information for a League of Legends match in progress
/// </summary>
public class CurrentGameInfoDto
{
    /// <summary>
    /// The ID of the game
    /// </summary>
    public long gameId { get; set; }

    /// <summary>
    /// The game type
    /// </summary>
    public string gameType { get; set; }

    /// <summary>
    /// The game start time represented in epoch milliseconds
    /// </summary>
    public long gameStartTime { get; set; }

    /// <summary>
    /// The ID of the map
    /// </summary>
    public long mapId { get; set; }

    /// <summary>
    /// The amount of time in seconds that has passed since the game started
    /// </summary>
    public long gameLength { get; set; }

    /// <summary>
    /// The ID of the platform on which the game is being played
    /// </summary>
    public string platformId { get; set; }

    /// <summary>
    /// The game mode
    /// </summary>
    public string gameMode { get; set; }

    /// <summary>
    /// Banned champion information
    /// </summary>
    public List<BannedChampionDto> bannedChampions { get; set; }

    /// <summary>
    /// The queue type (queue types are documented on the Game Constants page)
    /// </summary>
    public long gameQueueConfigId { get; set; }

    /// <summary>
    /// The observer information
    /// </summary>
    public ObserverDto observers { get; set; }

    /// <summary>
    /// The participant information
    /// </summary>
    public List<CurrentGameParticipantDto> participants { get; set; }
}
