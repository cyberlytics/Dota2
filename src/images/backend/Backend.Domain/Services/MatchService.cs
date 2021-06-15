using Backend.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Backend.Domain.Services
{
    public class MatchService : IMatchService
    {
        private readonly IMatchRepository _matchesRepository;

        public MatchService(IMatchRepository matchesRepository)
        {
            _matchesRepository = matchesRepository;
        }

        public Match Create(Match match)
        {
            return _matchesRepository.Create(match);
        }

        public List<Match> Get()
        {
            return _matchesRepository.Get();
        }

        public Match Get(long id)
        {
            return _matchesRepository.Get(id);
        }

        public void Remove(Match match)
        {
            _matchesRepository.Remove(match);
        }

        public void Remove(long id)
        {
            _matchesRepository.Remove(id);
        }

        public void Update(long id, Match match)
        {
            _matchesRepository.Update(id, match);
        }
    }
}
