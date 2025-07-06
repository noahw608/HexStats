namespace HexStats.Dto.League;

/// <summary>
/// Represents game customization object information for a League of Legends participant
/// </summary>
public class GameCustomizationObjectDto
{
    /// <summary>
    /// Category identifier for Game Customization
    /// </summary>
    public string category { get; set; }

    /// <summary>
    /// Game Customization content
    /// </summary>
    public string content { get; set; }
}
