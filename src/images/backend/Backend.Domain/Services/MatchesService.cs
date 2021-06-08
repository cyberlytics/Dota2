using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Backend.Domain.Models;
using Backend.Domain.Repositories;
using Newtonsoft.Json;

namespace Backend.Domain.Services
{
    public class MatchesService : IMatchesService
    {
        private readonly IMatchesRepository _repository;
        private const string apiUrl = "https://api.opendota.com/api/matches/";

        public MatchesService(IMatchesRepository repository)
        {
            _repository = repository;
        }

        public async Task<Match> RequestMatch(long id)
        {
            Match match = _repository.GetAll().Find(x => x.match_id == id);
            if (match == null)
            {
                HttpClient httpClient = new HttpClient();

                HttpResponseMessage responseMessage = await httpClient.GetAsync(apiUrl + id);
                string content = await responseMessage.Content.ReadAsStringAsync();
                match = JsonConvert.DeserializeObject<Match>(content, new JsonSerializerSettings {NullValueHandling = NullValueHandling.Ignore});
                _repository.Add(match);
            }
            
            return match;
        }

        public List<Match> GetAllMatches()
        {
            return _repository.GetAll();
        }
    }
}