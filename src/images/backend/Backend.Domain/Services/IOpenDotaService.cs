using System.Collections.Generic;
using System.Threading.Tasks;
using Backend.Domain.Models;

namespace Backend.Domain.Services
{
    public interface IOpenDotaService
    {
        Task<Match> RequestMatch(long id);
    }
}