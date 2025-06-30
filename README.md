# HexStats

A .NET 9 Discord bot application for tracking and displaying statistics.

## Features

- Discord bot integration using Discord.Net library
- Structured logging with Serilog
- Configuration management
- Service-oriented architecture

## Prerequisites

- .NET 9.0 SDK
- Discord Bot Token

## Getting Started

1. Clone the repository
2. Configure your Discord bot token in `appsettings.json`
3. Build and run the application:

```bash
dotnet build
dotnet run
```

## Configuration

Copy `appsettings.json` and create environment-specific configuration files:
- `appsettings.Development.json` for development settings
- `appsettings.Production.json` for production settings

**Note:** These files are ignored by git for security purposes.

## Project Structure

```
HexStats/
├── Src/
│   ├── Services/          # Service layer implementations
│   ├── Program.cs         # Application entry point
│   └── appsettings.json   # Base configuration
├── HexStats.csproj        # Project file
└── HexStats.sln          # Solution file
```

## Dependencies

- Discord.Net 3.17.4 - Discord API wrapper
- Microsoft.Extensions.Hosting 9.0.6 - Host builder and DI
- Microsoft.Extensions.Configuration 9.0.6 - Configuration management
- Serilog 4.3.0 - Structured logging

## Contributing

1. Fork the repository
2. Create a feature branch
3. Make your changes
4. Submit a pull request

## License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.
