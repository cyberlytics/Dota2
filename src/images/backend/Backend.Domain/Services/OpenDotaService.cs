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

        public async Task<string> FetchNewMatches(int number = 1)
        {
            //TODO: Für Debug. Kann später entfernt werden
            int cntPublic = 0;
            int cntDouble = 0;
            int cntRequest = 0;
            int cntNull = 0;
            var dtStart = DateTime.Now;

            List<Match> validMatches = new();

            //Public Matches anfordern bis gewünschte Anzahl neuer Matches vorhanden
            while (validMatches.Count < number)
            {
                //Hole 100 Public Matches
                var publicMatches = await FetchPublicMatches();

                cntPublic += publicMatches.Count;
                
                //Filter nach Ranked && All Draft
                List<Match> getMatches = new();
                getMatches.AddRange(publicMatches.Where(x => x.lobby_type == 7 && x.game_mode == 22 && _repository.Get(x.match_id) == null));

                //Komplette Matchdaten für alle gültigen Matches laden
                foreach (var getMatch in getMatches)
                {
                    //Hole gesamte Match Data
                    var requestedMatch = await FetchMatchData(getMatch.match_id);
                    cntRequest++;

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

            //TODO: Returnvalue für Debug. Kann später entfernt werden
            return $"New Matches: {validMatches.Count} | Public Ids: {cntPublic} | Requested Matches: {cntRequest} | Doppelte Ids: {cntDouble} | Nullwerte: {cntNull} | Time: {DateTime.Now - dtStart}";
        }

        public async Task<string> FetchNewMatchesAndParse(int number = 1)
        {
            //TODO: Returnvalue für Debug. Kann später entfernt werden
            int cntPublic = 0;
            int cntDouble = 0;
            int cntRequest = 0;
            int cntNull = 0;
            int cntParsed = 0;
            var dtStart = DateTime.Now;

            List<Match> validMatches = new();

            //Public Matches anfordern bis gewünschte Anzahl neuer Matches vorhanden
            while (validMatches.Count < number)
            {
                //Hole 100 Public Matches
                var publicMatches = await FetchPublicMatches();

                cntPublic += publicMatches.Count;

                //Filter nach Ranked && All Draft
                List<Match> getMatches = new();
                getMatches.AddRange(publicMatches.Where(x => x.lobby_type == 7 && x.game_mode == 22 && _repository.Get(x.match_id) == null));

                //Fordere Parse für alle IDs an
                foreach (var getMatch in getMatches)
                {
                    //Fordere Parse an
                    _ = await _openDotaApi.PostValue(apiUrlRequest + getMatch.match_id, new StringContent(""));
                    cntParsed++;
                }

                //Wartezeit für Parsing
                await Task.Delay(TimeSpan.FromMinutes(2));

                //Komplette Matchdaten für alle gültigen Matches laden
                foreach (var getMatch in getMatches)
                {
                    //Hole gesamte Match Data
                    var requestedMatch = await FetchMatchData(getMatch.match_id);
                    cntRequest++;

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

            //TODO: Returnvalue für Debug. Kann später entfernt werden
            return $"New Matches: {validMatches.Count} | Public Ids: {cntPublic} | Parse Requests: {cntParsed} | Requested Matches: {cntRequest} | Doppelte Ids: {cntDouble} | Nullwerte: {cntNull} | Time: {DateTime.Now - dtStart}";
        }

        public async Task<string> FetchAllMatchesForPlayer(long steam32Id, int limit = 100)
        {
            string apiUrlPlayerMatches = $"https://api.opendota.com/api/players/{steam32Id}/matches?lobby_type=7&game_mode=22";
            if (limit > 0)
            {
                apiUrlPlayerMatches = $"https://api.opendota.com/api/players/{steam32Id}/matches?lobby_type=7&game_mode=22&limit={limit}";
            }

            List<Match> playerMatches = new();
            List<Match> validMatches = new();

            //Hole alle Matches des Spielers
            var content = await _openDotaApi.GetValue(apiUrlPlayerMatches);
            if (!string.IsNullOrEmpty(content))
            {
                playerMatches = JsonConvert.DeserializeObject<List<Match>>(content, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
            }
            
            //Filter die Matches nach Parsed, Lobbytype und Gamemode
            playerMatches.RemoveAll(x => x.version == -1 || x.lobby_type != 7 || x.game_mode != 22);
            
            //Hole gesamte Matchdata für alle gültigen Matches
            var ids = playerMatches.Select(x => x.match_id).ToList();
            var getmatches = await FetchMatchData(ids);
            validMatches.AddRange(getmatches);
            //validMatches.AddRange(await FetchMatchData(playerMatches.Select(x => x.match_id).ToList()));

            //TODO: Spiele eines Spielers (getrennt) abspeichern? Verfälschung der Random Public Stats?
            foreach (var match in validMatches)
            {
                _repository.Create(match);
            }
            
            return $"{validMatches.Count} Matchdaten gespeichert.";
        }

        private async Task<List<Match>> FetchPublicMatches()
        {
            string apiUrlPublicMatches = "https://api.opendota.com/api/publicMatches";
            List<Match> ret = new();
            var content = await _openDotaApi.GetValue(apiUrlPublicMatches);
            if (!string.IsNullOrEmpty(content))
            {
                ret = JsonConvert.DeserializeObject<List<Match>>(content, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
            }

            return ret;
        }

        private async Task<Match> FetchMatchData(long id)
        {
            string apiUrlMatches = "https://api.opendota.com/api/matches/";
            Match ret = new();
            if (id > 0)
            {
                var content = await _openDotaApi.GetValue(apiUrlMatches + id);
                if (!string.IsNullOrEmpty(content))
                {
                    ret = JsonConvert.DeserializeObject<Match>(content, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
                }
            }

            return ret;
        }

        private async Task<List<Match>> FetchMatchData(List<long> ids)
        {
            string apiUrlMatches = "https://api.opendota.com/api/matches/";
            List<Match> ret = new();
            foreach (var id in ids)
            {
                if (id > 0)
                {
                    var content = await _openDotaApi.GetValue(apiUrlMatches + id);
                    if (!string.IsNullOrEmpty(content))
                    {
                        ret.Add(JsonConvert.DeserializeObject<Match>(content, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore }));
                    }
                }
            }

            return ret;
        }
    }
}