using System;
using System.Net;
using System.Net.Http;
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
            var httpContent = new StringContent(matchDtoJson, Encoding.UTF8, "application/json");
            var url = $"{pythonUrl}/writematch";
            Console.WriteLine(matchDtoJson);
            Console.WriteLine(url);

            var response = await _httpClient.PostAsync(url, httpContent);
            
            Console.WriteLine(response.StatusCode.ToString());

            return response.StatusCode;
        }
    }
}