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
    public class MatchesService : IMatchesService
    {
        private readonly IMatchesRepository _repository;
        private const string apiUrlMatches = "https://api.opendota.com/api/matches/";
        private const string apiUrlParsedMatches = "https://api.opendota.com/api/parsedMatches";

        public MatchesService(IMatchesRepository repository)
        {
            _repository = repository;
        }

        public async Task<Match> RequestMatch(long id)
        {
            Match match = _repository.GetAll().Find(x => x.match_id == id);
            if (match == null && id > 0)
            {
                HttpClient httpClient = new HttpClient();

                HttpResponseMessage responseMessage = await httpClient.GetAsync(apiUrlMatches + id);
                string content = await responseMessage.Content.ReadAsStringAsync();
                match = JsonConvert.DeserializeObject<Match>(content, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
                _repository.Add(match);
            }

            return match;
        }

        public async Task<List<Match>> RequestParsedMatches()
        {
            List<Match> matches = new();
            List<Match> database = GetAllMatches();

            HttpClient httpClient = new HttpClient();

            HttpResponseMessage responseMessage = await httpClient.GetAsync(apiUrlParsedMatches);
            string content = await responseMessage.Content.ReadAsStringAsync();
            List<MatchId> randomIds = JsonConvert.DeserializeObject<List<MatchId>>(content, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });

            foreach (MatchId id in randomIds)
            {
                //Prüft ob Match bereits vorhanden, ansonsten holt Match
                if (!database.Where(x => x.match_id == id.match_id).Any())
                {
                    matches.Add(await RequestMatch(id.match_id));
                    //Drosseln der Abfrage auf 1 Abfrage pro Sekunde (Limit: 60/Minute)
                    await Task.Delay(1000);
                }
            }

            return matches;
        }

        public List<Match> GetAllMatches()
        {
            return _repository.GetAll();
        }
    }
}