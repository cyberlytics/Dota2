using System.Collections.Generic;
using System.Threading.Tasks;
using Backend.Domain.Models;

namespace Backend.Domain.Services
{
    public interface IOpenDotaService
    {
        Task<List<long>> FetchNewMatches(int number = 1);

        Task<List<long>> FetchNewMatchesAndParse(int number = 1);

        Task<List<long>> FetchAllMatchesForPlayer(long steam32Id, int limit = 100);

        Task<List<long>> FetchRecentMatchesForPlayer(long steam32Id);

        Task<long> GetSteamIdByPersonaName(string name);
    }
}