using Discord.WebSocket;
using Microsoft.Extensions.Logging;
using Serilog;

namespace HexStats.Services;

public class ApplicationService : IApplicationService
{
    private readonly ILogger<ApplicationService> _logger;
    private static readonly DiscordSocketClient _discordClient;

    public ApplicationService(ILogger<ApplicationService> logger)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _logger.LogInformation("Logger initialized for ApplicationService");
    }

    public async Task RunAsync()
    {
        _logger.LogInformation("Application starting...");

        try
        {
            _logger.LogInformation("Application completed successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred during application execution");
            throw;
        }
    }
}