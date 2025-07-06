using HexStats.Data;
using HexStats.Enums;
using HexStats.Models;
using Microsoft.EntityFrameworkCore;

namespace HexStats.Repositories;

public class UserRepository : IUserRepository
{
    
    private readonly AppDbContext _dbContext;

    public UserRepository(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    
    public async Task<List<User>> GetAllUsersAsync()
    {
        return await _dbContext.Users.ToListAsync();
    }

    public async Task<User?> GetUserByIdAsync(int userId)
    {
        return await _dbContext.Users.FindAsync(userId)
            ?? throw new KeyNotFoundException("User not found with the specified ID.");
    }

    public async Task<User?> GetUserByDiscordUsernameAsync(string discordUsername)
    {
        return await _dbContext.Users.FirstOrDefaultAsync(u => 
            u.DiscordUsername == discordUsername )
            ?? throw new KeyNotFoundException("User not found with the specified Discord username and discriminator.");
    }

    public async Task<User?> GetUserByLeagueUsernameAsync(string leagueUsername, string leagueTagline, GameRegion region)
    {
        return await _dbContext.Users.FirstOrDefaultAsync(u =>
            u.LeagueUsername == leagueUsername && 
            u.LeagueTagline == leagueTagline && 
            u.LeagueGameRegion == region) 
            ?? throw new KeyNotFoundException("User not found with the specified League username, tagline, and region.");
    }
    
    public async Task<User?> GetUserByPuuidAsync(string puuid)
    {
        return await _dbContext.Users.FirstOrDefaultAsync(u => u.Puuid == puuid)
            ?? throw new KeyNotFoundException("User not found with the specified PUUID.");
    }

    public async Task AddUserAsync(User user)
    {
        if (user == null)
        {
            throw new ArgumentNullException(nameof(user), "User cannot be null.");
        }

        _dbContext.Users.Add(user);
        await _dbContext.SaveChangesAsync();
    }

    public async Task UpdateUserAsync(User user)
    {
        if (user == null)
        {
            throw new ArgumentNullException(nameof(user), "User cannot be null.");
        }

        _dbContext.Users.Update(user);
        await _dbContext.SaveChangesAsync();
    }

    public async Task DeleteUserAsync(int userId)
    {
        var user = await GetUserByIdAsync(userId);
        if (user == null)
        {
            throw new KeyNotFoundException("User not found with the specified ID.");
        }

        _dbContext.Users.Remove(user);
        await _dbContext.SaveChangesAsync();
    }

    public async Task<bool> UserExistsByIdAsync(int userId)
    {
        return await _dbContext.Users.AnyAsync(u => u.Id == userId);
    }
    
    public async Task<bool> UserExistsByLeagueUsernameAsync(string leagueUsername, string leagueTagline, GameRegion region)
    {
        return await _dbContext.Users.AnyAsync(u => 
            u.LeagueUsername == leagueUsername && 
            u.LeagueTagline == leagueTagline && 
            u.LeagueGameRegion == region);
    }
    
    public async Task<bool> UserExistsByDiscordUsernameAsync(string discordUsername)
    {
        return await _dbContext.Users.AnyAsync(u => u.DiscordUsername == discordUsername);
    }
}