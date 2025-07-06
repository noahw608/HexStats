using Discord;
using HexStats.Dto.League;
using HexStats.Enums;
using HexStats.Models;
using HexStats.Repositories;
using HexStats.Services.Interfaces;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;

namespace HexStats.Services;

public class GameStatusBackgroundService : BackgroundService
{
    
    private readonly ILogger<GameStatusBackgroundService> _logger;
    private readonly ILeagueService _leagueService;
    private readonly IUserRepository _userRepository;
    private readonly IDiscordService _discordService;

    public GameStatusBackgroundService(ILogger<GameStatusBackgroundService> logger, ILeagueService leagueService,
        IUserRepository userRepository, IDiscordService discordService)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _leagueService = leagueService ?? throw new ArgumentNullException(nameof(leagueService));
        _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
        _discordService = discordService ?? throw new ArgumentNullException(nameof(discordService));
    }
    
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        List<User> users = await _userRepository.GetAllUsersAsync();
        
        
        _logger.LogInformation("Starting game status check for {UserCount} users", users.Count);
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                Dictionary<CurrentGameInfoDto, User> currentGames = await _leagueService.GetCurrentGamesByPuuidAsync(users, stoppingToken);
                List<CurrentGameInfoDto> currentGamesInfo = new List<CurrentGameInfoDto>();
                // Create new map from gameId to list of users
                

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while checking game status");
            }

            await Task.Delay(TimeSpan.FromMinutes(5), stoppingToken);
        }
    }
}