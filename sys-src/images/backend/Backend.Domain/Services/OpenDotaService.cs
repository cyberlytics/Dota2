﻿using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Backend.Domain.Models;
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

        //Holt neue Public Matchdaten mit optionalen parsing bis Anzahl erreicht
        public async Task<List<long>> FetchNewMatches(int number = 1, bool parse = true, bool numberIsTarget = false)
        {
            List<Match> validMatches = new();

            //Public Matches anfordern bis gewünschte Anzahl neuer Matches vorhanden
            while (validMatches.Count < number)
            {
                //Hole 100 Public Matches
                var publicMatches = await FetchPublicMatches();

                //Filter nach Ranked && All Draft && bereits in Datenbank
                List<Match> getMatches = new();
                getMatches.AddRange(publicMatches.Where(x => x.lobby_type == 7 && x.game_mode == 22 && _repository.Get(x.match_id) == null));

                //Optionales Parsing
                if (parse)
                {
                    //Fordere Parse für alle IDs an
                    foreach (var getMatch in getMatches)
                    {
                        //Fordere Parse an
                        _ = await _openDotaApi.PostValue(apiUrlRequest + getMatch.match_id, new StringContent(""));
                    }

                    //Wartezeit für Parsing
                    await Task.Delay(TimeSpan.FromMinutes(2));
                }

                //Komplette Matchdaten für alle gültigen Matches laden
                foreach (var getMatch in getMatches)
                {
                    //Hole gesamte Match Data
                    var requestedMatch = await FetchMatchData(getMatch.match_id);

                    //Prüfe ob Match geparsed
                    if (requestedMatch.version != -1)
                    {
                        validMatches.Add(requestedMatch);

                        //Prüfe Abbruchbedingung
                        if (numberIsTarget && validMatches.Count == number)
                        {
                            break;
                        }
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
            //Grundsätzliches Limit auf 300 Matches (~ 5 Minuten), da Laufzeit sonst aufgrund des RateLimits unberechnbar wird (z.B. 2000 Matches dauern über 30 Minuten)
            string apiUrlAllPlayerMatches = $"https://api.opendota.com/api/players/{steam32Id}/matches?lobby_type=7&game_mode=22&limit=300";
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

        //Holt und parsed ein Match
        public async Task<Match> FetchMatch(long matchId)
        {
            //Prüft ob Match bereits in Datenbank
            if (_repository.Get(matchId) != null)
            {
                //Match bereits in Datenbank enthalten
                return _repository.Get(matchId);
            }

            var getMatch = await FetchMatchData(matchId);

            //Prüfe ob Lobby type und game Mode gültig
            if (getMatch.game_mode != 22 || getMatch.lobby_type != 7)
            {
                //Match ungültig
                return null;
            }

            //Prüft ob Match geparsed
            if (getMatch.version == -1)
            {
                //Forder Parse an
                _ = await _openDotaApi.PostValue("https://api.opendota.com/api/request/" + getMatch.match_id, new StringContent(""));

                int timeout = 0;

                //Warte bis Match geparsed
                while (getMatch.version == -1 && timeout < 10)
                {
                    timeout++;
                    await Task.Delay(TimeSpan.FromSeconds(30));
                    getMatch = await FetchMatchData(matchId);
                }

                //Prüfe ob Match erfolgreich geparsed
                if (getMatch.version == -1)
                {
                    //Timeout
                    return null;
                }
            }

            //Match geparsed und gütlig
            _repository.Create(getMatch);
            return getMatch;
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

            if (accounts?.Count > 0 && accounts.FirstOrDefault().personaname == name)
            {
                return accounts.FirstOrDefault().account_id;
            }
            else
            {
                return -1;
            }
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