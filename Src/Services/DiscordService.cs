using Microsoft.Extensions.Logging;

namespace HexStats.Services;

public class DiscordService : IDiscordService
{
    private readonly ILogger<DiscordService> _logger;

    public DiscordService(ILogger<DiscordService> logger)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _logger.LogInformation("Logger initialized for DiscordService");
    }
    
}