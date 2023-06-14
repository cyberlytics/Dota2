using System.Collections.Generic;
using System.Threading.Tasks;
using Backend.Domain.Models;

namespace Backend.Domain.Services
{
    public interface IOpenDotaService
    {
        Task<List<long>> FetchNewMatches(int number = 1, bool parse = true, bool numberIsTarget = false);

        Task<List<long>> FetchAllMatchesForPlayer(long steam32Id, int limit = 100);

        Task<List<long>> FetchRecentMatchesForPlayer(long steam32Id);

        Task<Match> FetchMatch(long matchId);

        Task<long> GetSteamIdByPersonaName(string name);
    }
}