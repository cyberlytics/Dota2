using System.Collections.Generic;
using System.Threading.Tasks;
using Backend.Domain.Models;

namespace Backend.Domain.Repositories
{
    public interface IMatchesRepository
    {
        void AddMatch(Match match);
        Match FindMatch(long id);
        List<Match> RequestMatches(int startId, int cnt);
        List<Match> FindUserMatches(long userId);
        int MaxId();
    }
}