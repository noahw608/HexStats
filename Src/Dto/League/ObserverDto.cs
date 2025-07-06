namespace HexStats.Dto.League;

/// <summary>
/// Represents observer information for a League of Legends match
/// </summary>
public class ObserverDto
{
    /// <summary>
    /// Key used to decrypt the spectator grid game data for playback
    /// </summary>
    public string encryptionKey { get; set; }
}
