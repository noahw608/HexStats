using System.Reflection;
using Discord.WebSocket;

namespace HexStats.Services.Interfaces;

public interface IInteractionFrameworkService
{
    Task InitializeAsync();
    Task RegisterCommandsAsync(bool isGlobal = true);
    Task HandleInteractionAsync(SocketInteraction interaction);
    Task AddModulesAsync(Assembly assembly);
    Task OnDiscordReadyAsync();
}