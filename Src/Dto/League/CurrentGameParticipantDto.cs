namespace HexStats.Dto.League;

/// <summary>
/// Represents participant information for a League of Legends match in progress
/// </summary>
public class CurrentGameParticipantDto
{
    /// <summary>
    /// The ID of the champion played by this participant
    /// </summary>
    public long championId { get; set; }

    /// <summary>
    /// Perks/Runes Reforged Information
    /// </summary>
    public PerksDto perks { get; set; }

    /// <summary>
    /// The ID of the profile icon used by this participant
    /// </summary>
    public long profileIconId { get; set; }

    /// <summary>
    /// Flag indicating whether or not this participant is a bot
    /// </summary>
    public bool bot { get; set; }

    /// <summary>
    /// The team ID of this participant, indicating the participant's team
    /// </summary>
    public long teamId { get; set; }

    /// <summary>
    /// The encrypted puuid of this participant
    /// </summary>
    public string puuid { get; set; }

    /// <summary>
    /// The ID of the first summoner spell used by this participant
    /// </summary>
    public long spell1Id { get; set; }

    /// <summary>
    /// The ID of the second summoner spell used by this participant
    /// </summary>
    public long spell2Id { get; set; }

    /// <summary>
    /// List of Game Customizations
    /// </summary>
    public List<GameCustomizationObjectDto> gameCustomizationObjects { get; set; }
}
