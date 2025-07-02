using System.Reflection;
using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using HexStats.Configuration;
using Microsoft.Extensions.Logging;
using Serilog;

namespace HexStats.Services;

public class InteractionFrameworkService : IInteractionFrameworkService
{
    private readonly ILogger<InteractionFrameworkService> _logger;
    private readonly InteractionService _interactionService;
    private readonly DiscordConfiguration _discordConfig;
    private readonly DiscordSocketClient _discordClient;
    private readonly IServiceProvider _services;
    
    public InteractionFrameworkService(ILogger<InteractionFrameworkService> logger, IDiscordService discordService, IServiceProvider services)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _discordConfig = discordService.Config ?? throw new ArgumentNullException(nameof(discordService));
        _discordClient = discordService.Client ?? throw new ArgumentNullException(nameof(discordService.Client));
        _interactionService = new InteractionService(discordService.Client, new InteractionServiceConfig
        {
            DefaultRunMode = Discord.Interactions.RunMode.Async,
            LogLevel = LogSeverity.Info
        });
        _services = services;
        
        // Subscribe to Discord Ready event to initialize and register commands
        discordService.Ready += OnDiscordReadyAsync;
    }

    public async Task InitializeAsync()
    {
        _discordClient.InteractionCreated += HandleInteractionAsync;
        _interactionService.Log += msg =>
        {
            _logger.LogInformation(msg.ToString());
            return Task.CompletedTask;
        };
        
        await AddModulesAsync(Assembly.GetEntryAssembly());
    }

    public async Task RegisterCommandsAsync(bool isGlobal = true)
    {
        if (isGlobal)
        {
            await _interactionService.RegisterCommandsGloballyAsync();
        }
        else
        {
            await _interactionService.RegisterCommandsToGuildAsync(_discordConfig.GuildId); // Add more verbose checking if needed
        }
    }

    public async Task HandleInteractionAsync(SocketInteraction interaction)
    {
        try
        {
            var context = new SocketInteractionContext(_discordClient, interaction);
            await _interactionService.ExecuteCommandAsync(context, _services);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error handling interaction: {InteractionId}", interaction.Id);
            if (interaction.Type == InteractionType.ApplicationCommand)
            {
                await interaction.RespondAsync("An application error occurred while processing your request.", ephemeral: true);
            }
        }
    }

    public async Task AddModulesAsync(Assembly assembly)
    {
        await _interactionService.AddModulesAsync(assembly, _services);
    }

    public async Task OnDiscordReadyAsync()
    {
        try
        {
            await InitializeAsync();
            await RegisterCommandsAsync(false);
            _logger.LogInformation("Interaction framework initialized and commands registered successfully.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to initialize interaction framework or register commands.");
        }
    }
}