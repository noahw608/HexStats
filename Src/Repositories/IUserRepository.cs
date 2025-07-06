using HexStats.Enums;
using HexStats.Models;

namespace HexStats.Repositories;

public interface IUserRepository
{
    Task<List<User>> GetAllUsersAsync();
    Task<User?> GetUserByIdAsync(int userId);
    Task<User?> GetUserByDiscordUsernameAsync(string discordUsername);
    Task<User?> GetUserByLeagueUsernameAsync(string leagueUsername, string leagueTagline, GameRegion region);
    Task<User?> GetUserByPuuidAsync(string puuid);
    Task AddUserAsync(User user);
    Task UpdateUserAsync(User user);
    Task DeleteUserAsync(int userId);
    Task<bool> UserExistsByIdAsync(int userId);
    Task<bool> UserExistsByLeagueUsernameAsync(string leagueUsername, string leagueTagline, GameRegion region);
    Task<bool> UserExistsByDiscordUsernameAsync(string discordUsername);
}