using System.Collections.Generic;
using Backend.Domain.Models;

namespace Backend.Domain.Services
{
    public interface IMatchesService
    {
        MatchDto FindMatch(long id);
        List<MatchDto> RequestMatches(int startId, int cnt);
    }
}