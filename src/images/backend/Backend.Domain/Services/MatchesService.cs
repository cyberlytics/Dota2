using System.Collections.Generic;
using Backend.Domain.Models;
using Backend.Domain.Repositories;

namespace Backend.Domain.Services
{
    public class MatchesService : IMatchesService
    {
        private readonly IMatchesLocalRepository _matchesRepository;

        public MatchesService(IMatchesLocalRepository matchesRepository)
        {
            _matchesRepository = matchesRepository;
        }

        public MatchDto FindMatch(long id)
        {
            Match match = _matchesRepository.FindMatch(id);

            if (match == null) return null;

            return new MatchDto(match);
        }

        public List<MatchDto> RequestMatches(int startId, int cnt)
        {
            List<Match> matches = _matchesRepository.RequestMatches(startId, cnt);
            List<MatchDto> matchDtos = new List<MatchDto>();
            
            foreach (var match in matches)
            {
                matchDtos.Add(new MatchDto(match));
            }

            return matchDtos;
        }
    }
}