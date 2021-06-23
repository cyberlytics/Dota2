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
        private readonly IMatchRepository _repository;
        private readonly IOpenDotaCallerService _openDotaApi;
        private const string apiUrlRequest = "https://api.opendota.com/api/request/";

        public OpenDotaService(IMatchRepository repository, IOpenDotaCallerService openDotaApi)
        {
            _repository = repository;
            _openDotaApi = openDotaApi;
        }

        //Holt neue Public Matchdaten bis Anzahl erreicht
        public async Task<List<long>> FetchNewMatches(int number = 1)
        {
            List<Match> validMatches = new();

            //Public Matches anfordern bis gewünschte Anzahl neuer Matches vorhanden
            while (validMatches.Count < number)
            {
                //Hole 100 Public Matches
                var publicMatches = await FetchPublicMatches();

                //Filter nach Ranked && All Draft
                List<Match> getMatches = new();
                getMatches.AddRange(publicMatches.Where(x => x.lobby_type == 7 && x.game_mode == 22 && _repository.Get(x.match_id) == null));

                //Komplette Matchdaten für alle gültigen Matches laden
                foreach (var getMatch in getMatches)
                {
                    //Hole gesamte Match Data
                    var requestedMatch = await FetchMatchData(getMatch.match_id);

                    //Prüfe ob Match geparsed
                    if (requestedMatch.version != -1)
                    {
                        validMatches.Add(requestedMatch);
                    }
                }
            }

            //Alle neuen Matches speichern
            foreach (var match in validMatches)
            {
                _repository.Create(match);
            }

            return validMatches.Select(x => x.match_id).ToList();
        }

        //Holt neue Public Matchdaten und Parsed bis Anzahl erreicht
        public async Task<List<long>> FetchNewMatchesAndParse(int number = 1)
        {
            List<Match> validMatches = new();

            //Public Matches anfordern bis gewünschte Anzahl neuer Matches vorhanden
            while (validMatches.Count < number)
            {
                //Hole 100 Public Matches
                var publicMatches = await FetchPublicMatches();

                //Filter nach Ranked && All Draft
                List<Match> getMatches = new();
                getMatches.AddRange(publicMatches.Where(x => x.lobby_type == 7 && x.game_mode == 22 && _repository.Get(x.match_id) == null));

                //Fordere Parse für alle IDs an
                foreach (var getMatch in getMatches)
                {
                    //Fordere Parse an
                    _ = await _openDotaApi.PostValue(apiUrlRequest + getMatch.match_id, new StringContent(""));
                }

                //Wartezeit für Parsing
                await Task.Delay(TimeSpan.FromMinutes(2));

                //Komplette Matchdaten für alle gültigen Matches laden
                foreach (var getMatch in getMatches)
                {
                    //Hole gesamte Match Data
                    var requestedMatch = await FetchMatchData(getMatch.match_id);

                    //Prüfe ob Match geparsed
                    if (requestedMatch.version != -1)
                    {
                        validMatches.Add(requestedMatch);
                    }
                }
            }

            //Alle neuen Matches speichern
            foreach (var match in validMatches)
            {
                _repository.Create(match);
            }

            return validMatches.Select(x => x.match_id).ToList();
        }

        //Holt eine Anzahl an Matches anhand einer Steam32 Id
        public async Task<List<long>> FetchAllMatchesForPlayer(long steam32Id, int limit = 10)
        {
            string apiUrlAllPlayerMatches = $"https://api.opendota.com/api/players/{steam32Id}/matches?lobby_type=7&game_mode=22";
            if (limit > 0)
            {
                apiUrlAllPlayerMatches = $"https://api.opendota.com/api/players/{steam32Id}/matches?lobby_type=7&game_mode=22&limit={limit}";
            }

            List<Match> playerMatches = new();
            List<Match> validMatches = new();

            //Hole alle Matches des Spielers
            var content = await _openDotaApi.GetValue(apiUrlAllPlayerMatches);
            if (!string.IsNullOrWhiteSpace(content))
            {
                playerMatches = JsonConvert.DeserializeObject<List<Match>>(content, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
            }

            //Filter die Matches nach Parsed, Lobbytype und Gamemode
            playerMatches.RemoveAll(x => x.version == -1 || x.lobby_type != 7 || x.game_mode != 22);

            //Hole gesamte Matchdata für alle gültigen Matches
            var ids = playerMatches.Select(x => x.match_id).ToList();
            var getmatches = await FetchMatchData(ids);
            validMatches.AddRange(getmatches);

            //Alle Matches des Spielers speichern
            foreach (var match in validMatches)
            {
                _repository.Create(match);
            }

            return ids;
        }

        //Holt die 20 letzten Matches anhand einer Steam32 Id
        public async Task<List<long>> FetchRecentMatchesForPlayer(long steam32Id)
        {
            string apiUrlRecentPlayerMatches = $"https://api.opendota.com/api/players/{steam32Id}/recentMatches";

            List<Match> playerMatches = new();
            List<Match> validMatches = new();

            //Hole Recent Matches des Spielers
            var content = await _openDotaApi.GetValue(apiUrlRecentPlayerMatches);
            if (!string.IsNullOrWhiteSpace(content))
            {
                playerMatches = JsonConvert.DeserializeObject<List<Match>>(content, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
            }

            //Filter die Matches nach Parsed, Lobbytype und Gamemode
            playerMatches.RemoveAll(x => x.version == -1 || x.lobby_type != 7 || x.game_mode != 22);

            //Hole gesamte Matchdata für alle gültigen Matches
            var ids = playerMatches.Select(x => x.match_id).ToList();
            var getmatches = await FetchMatchData(ids);
            validMatches.AddRange(getmatches);

            foreach (var match in validMatches)
            {
                _repository.Create(match);
            }

            return ids;
        }

        //Holt die Steam32Id anhand eines Usernames
        public async Task<long> GetSteamIdByPersonaName(string name)
        {
            List<Account> accounts = new();

            if (!string.IsNullOrWhiteSpace(name))
            {
                string apiUrlSearch = $"https://api.opendota.com/api/search?q={name}";

                var content = await _openDotaApi.GetValue(apiUrlSearch);
                if (!string.IsNullOrWhiteSpace(content))
                {
                    accounts = JsonConvert.DeserializeObject<List<Account>>(content, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
                }
            }

            return accounts.FirstOrDefault().account_id;
        }

        //Holt 100 Public Matches
        private async Task<List<Match>> FetchPublicMatches()
        {
            string apiUrlPublicMatches = "https://api.opendota.com/api/publicMatches";
            List<Match> ret = new();
            var content = await _openDotaApi.GetValue(apiUrlPublicMatches);
            if (!string.IsNullOrWhiteSpace(content))
            {
                ret = JsonConvert.DeserializeObject<List<Match>>(content, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
            }

            return ret;
        }

        //Holt Matchdaten anhand einer Id
        private async Task<Match> FetchMatchData(long id)
        {
            string apiUrlMatches = "https://api.opendota.com/api/matches/";
            Match ret = new();
            if (id > 0)
            {
                var content = await _openDotaApi.GetValue(apiUrlMatches + id);
                if (!string.IsNullOrWhiteSpace(content))
                {
                    ret = JsonConvert.DeserializeObject<Match>(content, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
                }
            }

            return ret;
        }

        //Holt Matchdaten anhand einer Liste von Ids
        private async Task<List<Match>> FetchMatchData(List<long> ids)
        {
            string apiUrlMatches = "https://api.opendota.com/api/matches/";
            List<Match> ret = new();
            foreach (var id in ids)
            {
                if (id > 0)
                {
                    var content = await _openDotaApi.GetValue(apiUrlMatches + id);
                    if (!string.IsNullOrWhiteSpace(content))
                    {
                        ret.Add(JsonConvert.DeserializeObject<Match>(content, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore }));
                    }
                }
            }

            return ret;
        }
    }
}