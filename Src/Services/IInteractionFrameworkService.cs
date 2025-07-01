using System.Reflection;
using Discord.WebSocket;

namespace HexStats.Services;

public interface IInteractionFrameworkService
{
    Task InitializeAsync();
    Task RegisterCommandsAsync(bool isGlobal = true);
    Task HandleInteractionAsync(SocketInteraction interaction);
    Task AddModulesAsync(Assembly assembly);
}