using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Backend.Domain.Models;
using Backend.Domain.Repositories;
using Newtonsoft.Json;
using System.Linq;

namespace Backend.Domain.Services
{
    public class OpenDotaService : IOpenDotaService
    {
        private readonly IMatchesRepository _repository;
        private const string apiUrlMatches = "https://api.opendota.com/api/matches/";
        private const string apiUrlParsedMatches = "https://api.opendota.com/api/parsedMatches";

        public OpenDotaService(IMatchesRepository repository)
        {
            _repository = repository;
        }

        public async Task<Match> RequestMatch(long id)
        {
            Match match = _repository.FindMatch(id);
            if (match == null && id > 0)
            {
                HttpClient httpClient = new HttpClient();

                HttpResponseMessage responseMessage = await httpClient.GetAsync(apiUrlMatches + id);
                string content = await responseMessage.Content.ReadAsStringAsync();
                match = JsonConvert.DeserializeObject<Match>(content, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
                _repository.AddMatch(match);
            }

            return match;
        }
    }
}