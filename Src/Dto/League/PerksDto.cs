namespace HexStats.Dto.League;

/// <summary>
/// Represents perks/runes information for a League of Legends participant
/// </summary>
public class PerksDto
{
    /// <summary>
    /// IDs of the perks/runes assigned
    /// </summary>
    public List<long> perkIds { get; set; }

    /// <summary>
    /// Primary runes path
    /// </summary>
    public long perkStyle { get; set; }

    /// <summary>
    /// Secondary runes path
    /// </summary>
    public long perkSubStyle { get; set; }
}
