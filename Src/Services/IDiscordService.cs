using Discord.WebSocket;

namespace HexStats.Services;

public interface IDiscordService
{
    Task StartAsync(CancellationToken cancellationToken = default);
    Task StopAsync(CancellationToken cancellationToken = default);
    Task WaitForReadyAsync(TimeSpan? timeout = null);
    Task SendMessageAsync(ulong channelId, string message);
    Task ListAvailableChannelsAsync();
    Task<bool> IsConnectedAsync();
    
    event Func<SocketMessage, Task> MessageReceived;
    event Func<Task> Ready;
}