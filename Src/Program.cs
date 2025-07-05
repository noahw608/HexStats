using Discord;
using Discord.WebSocket;
using HexStats.Configuration;
using HexStats.Data;
using HexStats.Repositories;
using HexStats.Services;
using HexStats.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
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
                services.AddDbContext<AppDbContext>(options => options.UseSqlite(context.Configuration.GetConnectionString("DefaultConnection")));
                services.AddScoped<IUserRepository, UserRepository>();
                
                
                services.Configure<DiscordConfiguration>(context.Configuration.GetSection("Discord"));
                services.PostConfigure<DiscordConfiguration>(config =>
                {
                    config.Token = Environment.GetEnvironmentVariable("DISCORD_TOKEN")
                                   ?? throw new InvalidOperationException("DISCORD_TOKEN environment variable is not set");
                });
                services.AddSingleton<IDiscordService, DiscordService>();
                
                services.Configure<LeagueConfiguration>(context.Configuration.GetSection("League"));
                services.PostConfigure<LeagueConfiguration>(config =>
                {
                    config.Key = Environment.GetEnvironmentVariable("LEAGUE_KEY")
                                    ?? throw new InvalidOperationException("LEAGUE_KEY environment variable is not set");
                });
                services.AddSingleton<ILeagueService, LeagueService>();
                
                services.AddSingleton<IInteractionFrameworkService, InteractionFrameworkService>();
                services.AddSingleton<IApplicationService, ApplicationService>();
            })
            .Build();
        
        using (host)
        {
            try
            {
                using var scope = host.Services.CreateScope();
                var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                await dbContext.Database.MigrateAsync();
                
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