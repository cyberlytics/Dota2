using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Backend.Domain.Models;
using Newtonsoft.Json;

namespace Backend.Domain.Services
{
    public class JupyterService : IJupyterService
    {
        private readonly IMatchRepository _matchRepository;
        private HttpClient _httpClient;
        private static string pythonUrl = "http://host.docker.internal:8898";

        public JupyterService(IMatchRepository matchRepository)
        {
            _matchRepository = matchRepository;
            _httpClient = new HttpClient();
        }

        public async Task<HttpStatusCode> WriteMatchAsync(long matchId)
        {
            var match = _matchRepository.Get(matchId);
            var matchDto = new MatchDto(match);
            var matchDtoJson = JsonConvert.SerializeObject(matchDto);
            var url = $"{pythonUrl}/writematch";
            
            HttpResponseMessage response;

            using (var request =
                new HttpRequestMessage(new HttpMethod("POST"), url))
            {
                request.Content = new StringContent(matchDtoJson);
                request.Content.Headers.ContentType = MediaTypeHeaderValue.Parse("application/json");

                response = await _httpClient.SendAsync(request);
            }

            return response.StatusCode;
        }
    }
}