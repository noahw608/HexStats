using Discord;
using Discord.WebSocket;
using HexStats.Configuration;
using HexStats.Services;
using Serilog;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace HexStats;

class Program
{
    static async Task Main(string[] args)
    {
        
        var configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("DOTNET_ENVIRONMENT") ?? "Production"}.json", optional: true)
            .AddEnvironmentVariables()
            .AddCommandLine(args)
            .Build();
        
        
        Log.Logger = new LoggerConfiguration()
            .ReadFrom.Configuration(configuration)
            .CreateLogger();
        
        var host = Host.CreateDefaultBuilder(args)
            .UseSerilog()
            .ConfigureServices((context, services) =>
            {
                
                services.Configure<DiscordConfiguration>(context.Configuration.GetSection("Discord"));
                
                services.AddSingleton<IDiscordService, DiscordService>();
                services.AddSingleton<IInteractionFrameworkService, InteractionFrameworkService>();
                services.AddSingleton<IApplicationService, ApplicationService>();
            })
            .Build();
        
        using (host)
        {
            try
            {
                var app = host.Services.GetRequiredService<IApplicationService>();
                await app.RunAsync();
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "Application terminated unexpectedly");
                throw;
            }
            finally
            {
                await host.StopAsync();
            }
        }
        
    }
}