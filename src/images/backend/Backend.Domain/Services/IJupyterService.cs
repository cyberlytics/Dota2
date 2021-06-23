using System.Net;
using System.Threading.Tasks;

namespace Backend.Domain.Services
{
    public interface IJupyterService
    {
        public Task<HttpStatusCode> WriteMatchAsync(long matchId);
    }
}