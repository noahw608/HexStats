using Discord.WebSocket;
using HexStats.Services.Interfaces;
using Microsoft.Extensions.Logging;
using Serilog;

namespace HexStats.Services;

public class ApplicationService : IApplicationService
{
    private readonly ILogger<ApplicationService> _logger;
    private readonly IDiscordService _discordService;
    private readonly IInteractionFrameworkService _interactionFrameworkService;

    public ApplicationService(ILogger<ApplicationService> logger, IDiscordService discordService, IInteractionFrameworkService interactionFrameworkService)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _logger.LogInformation("Logger initialized for ApplicationService");
        _discordService = discordService ?? throw new ArgumentNullException(nameof(discordService));
        _logger.LogInformation("DiscordService initialized for ApplicationService");
        _interactionFrameworkService = interactionFrameworkService ?? throw new ArgumentNullException(nameof(interactionFrameworkService));
        _logger.LogInformation("InteractionFrameworkService initialized for ApplicationService");
    }

    public async Task RunAsync()
    {
        _logger.LogInformation("Application starting...");

        try
        {
            
            _discordService.Ready += OnDiscordReady;
            _discordService.MessageReceived += OnMessageReceived;
            
            await _discordService.StartAsync();
            
            _logger.LogInformation("Application started successfully. Press Ctrl+C to stop.");

            await _discordService.WaitForReadyAsync();

            await _discordService.SendMessageAsync(_discordService.Config.ChannelId , "Hello from my application.");
            
            await Task.Delay(-1);
            
            _logger.LogInformation("Application completed successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred during application execution");
            throw;
        }
    }
    
    private Task OnDiscordReady()
    {
        _logger.LogInformation("Discord bot is ready and connected!");
        return Task.CompletedTask;
    }

    private async Task OnMessageReceived(SocketMessage message)
    {
        if (message.Content.StartsWith("!hello"))
        {
            await _discordService.SendMessageAsync(message.Channel.Id, $"Hello {message.Author.Mention}!");
        }
        else if (message.Content.StartsWith("!ping"))
        {
            await _discordService.SendMessageAsync(message.Channel.Id, "Pong!");
        }
    }
}