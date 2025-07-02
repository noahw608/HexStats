using Discord;
using Discord.WebSocket;
using HexStats.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace HexStats.Services;

public class DiscordService : IDiscordService
{
    private readonly ILogger<DiscordService> _logger;
    private readonly DiscordSocketClient _discordClient;
    private readonly DiscordConfiguration _config;
    private readonly TaskCompletionSource<bool> _readyTaskCompletionSource;

    public DiscordService(ILogger<DiscordService> logger, IOptions<DiscordConfiguration> config)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _config = config.Value ?? throw new ArgumentNullException(nameof(config));
        _readyTaskCompletionSource = new TaskCompletionSource<bool>();
        
        var intents = ParseIntents(_config.Intents);


        _discordClient = new DiscordSocketClient(new DiscordSocketConfig
        {
            GatewayIntents = intents,
            LogLevel = LogSeverity.Info
        });
        
        _discordClient.Log += LogAsync;
        _discordClient.Ready += OnReadyAsync;
        _discordClient.MessageReceived += OnMessageReceivedAsync;
    }
    
    public DiscordSocketClient Client => _discordClient;
    public DiscordConfiguration Config => _config;
    
    public async Task StartAsync(CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Starting Discord service...");
        
        // Get token from environment variable first, fall back to configuration if needed
        var token = Environment.GetEnvironmentVariable("DISCORD_TOKEN");
        
        // If environment variable is not set, try to use configuration token
        if (string.IsNullOrEmpty(token))
        {
            token = _config.Token;
            _logger.LogInformation("Using Discord token from configuration (consider using DISCORD_TOKEN environment variable)");
        }
        else
        {
            _logger.LogInformation("Using Discord token from DISCORD_TOKEN environment variable");
        }
        
        // Validate that we have a token from either source
        if (string.IsNullOrEmpty(token) || token == "YOUR_BOT_TOKEN_HERE" || token == "${DISCORD_TOKEN}")
        {
            _logger.LogError("Discord bot token is not configured. Set DISCORD_TOKEN environment variable or Discord:Token configuration.");
            throw new InvalidOperationException("Discord bot token is not configured");
        }
        
        await _discordClient.LoginAsync(TokenType.Bot, token);
        await _discordClient.StartAsync();
        
        _logger.LogInformation("Discord service started successfully");
    }

    public async Task StopAsync(CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Stopping Discord service...");
        await _discordClient.StopAsync();
        await _discordClient.LogoutAsync();
        _logger.LogInformation("Discord service stopped");
    }
    
    public async Task WaitForReadyAsync(TimeSpan? timeout = null)
    {
        var timeoutTask = timeout.HasValue 
            ? Task.Delay(timeout.Value) 
            : Task.Delay(Timeout.Infinite);
            
        var completedTask = await Task.WhenAny(_readyTaskCompletionSource.Task, timeoutTask);
        
        if (completedTask == timeoutTask)
        {
            throw new TimeoutException("Discord client did not become ready within the specified timeout");
        }
        
        await _readyTaskCompletionSource.Task;
    }

    public async Task SendMessageAsync(ulong channelId, string message)
    {
        if (_discordClient.GetChannel(channelId) is IMessageChannel channel)
        {
            await channel.SendMessageAsync(message);
            _logger.LogInformation("Message sent to channel {ChannelId}", channelId);
        }
        else
        {
            _logger.LogWarning("Channel {ChannelId} not found", channelId);
        }
    }
    
    public async Task ListAvailableChannelsAsync()
    {
        _logger.LogInformation("Listing available channels...");
    
        foreach (var guild in _discordClient.Guilds)
        {
            _logger.LogInformation("Guild: {GuildName} (ID: {GuildId})", guild.Name, guild.Id);
        
            foreach (var channel in guild.TextChannels)
            {
                var permissions = channel.GetPermissionOverwrite(guild.CurrentUser);
                _logger.LogInformation("  Channel: #{ChannelName} (ID: {ChannelId}) - Permissions: {Permissions}", 
                    channel.Name, channel.Id, permissions?.ToString() ?? "No specific permissions");
            }
        }
    }

    public Task<bool> IsConnectedAsync()
    {
        return Task.FromResult(_discordClient?.ConnectionState == ConnectionState.Connected);
    }
    
    private Task LogAsync(LogMessage log)
    {
        var logLevel = log.Severity switch
        {
            LogSeverity.Critical => LogLevel.Critical,
            LogSeverity.Error => LogLevel.Error,
            LogSeverity.Warning => LogLevel.Warning,
            LogSeverity.Info => LogLevel.Information,
            LogSeverity.Verbose => LogLevel.Debug,
            LogSeverity.Debug => LogLevel.Trace,
            _ => LogLevel.Information
        };

        _logger.Log(logLevel, log.Exception, "[Discord] {Message}", log.Message);
        return Task.CompletedTask;
    }

    private async Task OnReadyAsync()
    {
        _logger.LogInformation("Discord bot is ready! Logged in as {Username}#{Discriminator}", 
            _discordClient.CurrentUser.Username, _discordClient.CurrentUser.Discriminator);
        
        // List all available channels for debugging
        // await ListAvailableChannelsAsync();

        _readyTaskCompletionSource.TrySetResult(true);
        
        Ready?.Invoke();
    }

    private async Task OnMessageReceivedAsync(SocketMessage message)
    {
        if (message.Author.IsBot) return;
        
        _logger.LogDebug("Message received from {Author}: {Content}", 
            message.Author.Username, message.Content);
        
        MessageReceived?.Invoke(message);
        return;
    }

    private GatewayIntents ParseIntents(string intentsString)
    {
        if (string.IsNullOrEmpty(intentsString))
            return GatewayIntents.AllUnprivileged;

        var intents = GatewayIntents.None;
        var intentNames = intentsString.Split(',', StringSplitOptions.RemoveEmptyEntries);
        
        _logger.LogInformation("Parsing intents: {IntentsString}", intentsString);
        
        foreach (var intentName in intentNames)
        {
            if (Enum.TryParse<GatewayIntents>(intentName.Trim(), true, out var intent))
            {
                intents |= intent;
                _logger.LogInformation("Added intent: {Intent}", intent);
            }
            else
            {
                _logger.LogWarning("Failed to parse intent: {IntentName}", intentName.Trim());
            }
        }
        
        _logger.LogInformation("Final intents: {Intents}", intents);
        return intents;
    }
    
    public event Func<SocketMessage, Task> MessageReceived;
    public event Func<Task> Ready;
    
}