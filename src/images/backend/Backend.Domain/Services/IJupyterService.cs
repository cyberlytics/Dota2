using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Backend.Domain.Models;

namespace Backend.Domain.Services
{
    public interface IJupyterService
    {
        public Task<HttpStatusCode> WriteMatchAsync(long matchId);
        public Task<HttpStatusCode> DeleteMatchAsync(long matchId);
        public Task<HttpStatusCode> TrainModelAsync(string model_name);
        public Task<PredictionResultDto> PredictAsync(long matchId, string model_name);
    }
}