using System.Collections.Generic;
using System.Threading.Tasks;
using Backend.Domain.Models;

namespace Backend.Domain.Services
{
    public interface IMatchesService
    {
        Task<Match> RequestMatch(long id);
        public List<Match> GetAllMatches();
    }
}