using System.Collections.Generic;
using System.Threading.Tasks;
using Backend.Domain.Models;

namespace Backend.Domain.Services
{
    public interface IOpenDotaService
    {
        Task<string> FetchNewMatches(int number = 1);

        Task<string> FetchNewMatchesAndParse(int number = 1);

        Task<string> FetchAllMatchesForPlayer(long steam32Id, int limit = 100);
    }
}