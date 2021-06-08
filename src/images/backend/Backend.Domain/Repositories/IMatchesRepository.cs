using System.Collections.Generic;
using System.Threading.Tasks;
using Backend.Domain.Models;

namespace Backend.Domain.Repositories
{
    public interface IMatchesRepository
    {
        void Add(Match match);
        List<Match> GetAll();
    }
}