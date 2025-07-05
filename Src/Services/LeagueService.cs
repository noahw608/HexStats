using System.Text.Json;
using Discord;
using HexStats.Configuration;
using HexStats.Dto.League;
using HexStats.Enums;
using HexStats.Services.Interfaces;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace HexStats.Services;

public class LeagueService : ILeagueService
{
 
    private readonly ILogger<LeagueService> _logger;
    private readonly LeagueConfiguration _configuration;
    private readonly HttpClient _httpClient;
    
    
    public LeagueService(IOptions<LeagueConfiguration> config, ILogger<LeagueService> logger)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _configuration = config.Value ?? throw new ArgumentNullException(nameof(config));
        
        _httpClient = new HttpClient();
    }
    
    public LeagueConfiguration Configuration => _configuration;
    
    public async Task<AccountDto> GetRiotAccountByLeagueNameAsync(string leagueName, string tagLine, AccountRegion accountRegion,
        CancellationToken cancellationToken = default)
    {

        string url =
            $"https://{accountRegion}.api.riotgames.com/riot/account/v1/accounts/by-riot-id/{leagueName}/{tagLine}?api_key={_configuration.Key}";
        var response = await _httpClient.GetAsync(url, cancellationToken);

        if (response.IsSuccessStatusCode == false)
        {
            _logger.LogError("Failed to retrieve account information for {LeagueName}#{TagLine} from {Region}. Status code: {StatusCode}",
                leagueName, tagLine, accountRegion, response.StatusCode);
            throw new HttpRequestException($"Failed to retrieve account information: {response.ReasonPhrase}");
        }
        
        var result = await response.Content.ReadAsStringAsync(cancellationToken);
        var accountDto = JsonSerializer.Deserialize<AccountDto>(result);
        
        if (accountDto == null)
        {
            _logger.LogError("Failed to deserialize account information for {LeagueName}#{TagLine} from {Region}. Response: {Response}",
                leagueName, tagLine, accountRegion, result);
            throw new JsonException("Failed to deserialize account information");
        }
        
        return accountDto;
    }

    public async Task<SummonerDto> GetSummonerByPuuidAsync(string puuid, GameRegion gameRegion, CancellationToken cancellationToken = default)
    {
        string url =
            $"https://{gameRegion}.api.riotgames.com/lol/summoner/v4/summoners/by-puuid/{puuid}?api_key={_configuration.Key}";
        var response = await _httpClient.GetAsync(url, cancellationToken);
        
        if (response.IsSuccessStatusCode == false)
        {
            _logger.LogError("Failed to retrieve summoner information for PUUID {Puuid} from {Region}. Status code: {StatusCode}",
                puuid, gameRegion, response.StatusCode);
            throw new HttpRequestException($"Failed to retrieve summoner information: {response.ReasonPhrase}");
        }
        
        var result = await response.Content.ReadAsStringAsync(cancellationToken);
        var summonerDto = JsonSerializer.Deserialize<SummonerDto>(result);
        
        if (summonerDto == null)
        {
            _logger.LogError("Failed to deserialize summoner information for PUUID {Puuid} from {Region}. Response: {Response}",
                puuid, gameRegion, result);
            throw new JsonException("Failed to deserialize summoner information");
        }
        
        return summonerDto;
    }
    
    public static AccountRegion MapGameRegionToAccountRegion(GameRegion gameRegion)
    {
        switch (gameRegion)
        {
            case GameRegion.NA1:
                return AccountRegion.Americas;
                break;
            case GameRegion.BR1:
                return AccountRegion.Americas;
                break;
            case GameRegion.LA1:
                return AccountRegion.Americas;
                break;
            case GameRegion.LA2:
                return AccountRegion.Americas;
                break;
            case GameRegion.KR:
                return AccountRegion.Asia;
                break;
            case GameRegion.JP1:
                return AccountRegion.Asia;
                break;
            case GameRegion.OC1:
                return AccountRegion.Asia;
                break;
            case GameRegion.PH2:
                return AccountRegion.Asia;
                break;
            case GameRegion.SG2:
                return AccountRegion.Asia;
                break;
            case GameRegion.TH2:
                return AccountRegion.Asia;
                break;
            case GameRegion.TW2:
                return AccountRegion.Asia;
                break;
            case GameRegion.VN2:
                return AccountRegion.Asia;
                break;
            case GameRegion.EUN1:
                return AccountRegion.Europe;
                break;
            case GameRegion.EUW1:
                return AccountRegion.Europe;
                break;
            case GameRegion.TR1:
                return AccountRegion.Europe;
                break;
            case GameRegion.RU:
                return AccountRegion.Europe;
                break;
            default:
                return AccountRegion.Americas;
                break;
        }
    }
}