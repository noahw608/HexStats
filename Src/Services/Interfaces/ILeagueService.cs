using HexStats.Configuration;
using HexStats.Dto.League;
using HexStats.Enums;
using HexStats.Models;

namespace HexStats.Services.Interfaces;

public interface ILeagueService
{

    LeagueConfiguration Configuration { get; }

    Task<AccountDto> GetRiotAccountByLeagueNameAsync(string leagueName, string tagLine, AccountRegion accountRegion, CancellationToken cancellationToken = default);
    
    Task<SummonerDto> GetSummonerByPuuidAsync(string puuid, GameRegion gameRegion, CancellationToken cancellationToken = default);
    
    Task<CurrentGameInfoDto?> GetCurrentGameByPuuidAsync(string puuid, GameRegion gameRegion, CancellationToken cancellationToken = default);
    
    Task<Dictionary<CurrentGameInfoDto, User>> GetCurrentGamesByPuuidAsync(List<User> users, CancellationToken cancellationToken = default);
}