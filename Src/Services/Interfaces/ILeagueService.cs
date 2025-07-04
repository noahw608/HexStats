using HexStats.Dto.League;
using HexStats.Enums;

namespace HexStats.Services.Interfaces;

public interface ILeagueService
{
    Task<AccountDto> GetRiotAccountByLeagueNameAsync(string leagueName, string tagLine, AccountRegion accountRegion, CancellationToken cancellationToken = default);
    
    Task<SummonerDto> GetSummonerByPuuidAsync(string puuid, GameRegion gameRegion, CancellationToken cancellationToken = default);
}