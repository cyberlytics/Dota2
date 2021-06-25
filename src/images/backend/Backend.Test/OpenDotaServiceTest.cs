using System.Collections.Generic;
using NUnit.Framework;
using Backend.Domain.Services;
using Backend.Domain.Models;
using System.Threading.Tasks;

namespace Backend.Test
{
    [TestFixture]
    public class OpenDotaServiceTest
    {
        private MatchRepository _matchRepository;
        private MatchstoreDatabaseSettings _matchstoreDatabaseSettings;
        private OpenDotaCallerService _openDotaApi;
        private OpenDotaService _openDotaService;

        // Testvorbereitungen
        [SetUp]
        public void Setup()
        {
            _matchstoreDatabaseSettings = new MatchstoreDatabaseSettings()
            {
                MatchesCollectionName = "Matches",
                ConnectionString = "mongodb://localhost:27017",
                DatabaseName = "MatchstoreDb"
            };
            _matchRepository = new MatchRepository(_matchstoreDatabaseSettings);

            _openDotaApi = new OpenDotaCallerService();

            _openDotaService = new OpenDotaService(_matchRepository, _openDotaApi);
        }

        [Test]
        public async Task FetchNewMatches_Matches_Abrufen_ohneParse()
        {
            List<Match> matches = _matchRepository.Get();

            int currentMatchCount = matches.Count;

            await _openDotaService.FetchNewMatches(1, false);

            // Mindestens ein neues Match muss vorliegen
            Assert.That(_matchRepository.Get().Count > currentMatchCount);
        }

        [Test]
        public async Task FetchNewMatches_Matches_Abrufen_mitParse()
        {
            List<Match> matches = _matchRepository.Get();

            int currentMatchCount = matches.Count;

            await _openDotaService.FetchNewMatches(1, true);

            // Mindestens ein neues Match muss vorliegen
            Assert.That(_matchRepository.Get().Count > currentMatchCount);
        }

        [Test]
        public async Task FetchNewMatches_Null_Abrufen()
        {
            int matchCount = _matchRepository.Get().Count;

            await _openDotaService.FetchNewMatches(0);

            // Kein neues Match darf vorliegen
            Assert.That(_matchRepository.Get().Count == matchCount);
        }

        [Test]
        public async Task FetchAllMatchesForPlayer_Spieler_Matches_Abrufen()
        {
            int steamId = 114297134;

            int limit = 2;

            int currentMatchCount = _matchRepository.Get().Count;

            // Max. limit Matches abrufen
            await _openDotaService.FetchAllMatchesForPlayer(steamId, limit);

            List<Match> matches = _matchRepository.Get();

            // Matchzahl muss genau um festgelegtes Limit erhoeht sein
            Assert.That(matches.Count == currentMatchCount + limit);

            int occurenceCount = 0;

            // Zahl der gespeicherten Matches mit Beteiligung
            // des gesuchten Spielers
            foreach (Match match in matches)
            {
                foreach (Match.Player player in match.players)
                {
                    if (player.account_id == steamId)
                    {
                        occurenceCount++;
                        break;
                    }
                }
            }

            // Mindestens so viele Matches mit diesem Spieler wie abgefragt wurden
            Assert.That(occurenceCount >= limit);
        }

        [Test]
        public async Task FetchAllMatchesForPlayer_Falscher_Spieler_Matches_Abrufen()
        {
            int matchCount = _matchRepository.Get().Count;

            // Nicht existierende Account-ID
            await _openDotaService.FetchAllMatchesForPlayer(1234567890, 2);

            // Matchzahl muss gleich bleiben
            Assert.That(_matchRepository.Get().Count == matchCount);
        }

        [Test]
        public async Task FetchRecentMatchesForPlayer_Spieler_Matches_Abrufen()
        {
            int steamId = 114297134;

            int currentMatchCount = _matchRepository.Get().Count;

            // Max. limit Matches abrufen
            await _openDotaService.FetchRecentMatchesForPlayer(steamId);

            List<Match> matches = _matchRepository.Get();

            // Matchzahl muss erhoeht sein
            Assert.That(matches.Count >= currentMatchCount);

            int occurenceCount = 0;

            // Zahl der gespeicherten Matches mit Beteiligung
            // des gesuchten Spielers
            foreach (Match match in matches)
            {
                foreach (Match.Player player in match.players)
                {
                    if (player.account_id == steamId)
                    {
                        occurenceCount++;
                        break;
                    }
                }
            }

            // Matches mit Spielerbeteiligung vorhanden
            Assert.That(occurenceCount > 0);
        }

        [Test]
        public async Task FetchRecentMatchesForPlayer_Falscher_Spieler_Matches_Abrufen()
        {
            int matchCount = _matchRepository.Get().Count;

            // Nicht existierende Account-ID
            await _openDotaService.FetchRecentMatchesForPlayer(1234567890);

            // Matchzahl muss gleich bleiben
            Assert.That(_matchRepository.Get().Count == matchCount);
        }

        [Test]
        public async Task FetchMatch_GültigesMatch()
        {
            long id = 6058011300;

            // gültiges Match
            Match ret = await _openDotaService.FetchMatch(id);

            // Rückgabe muss ungleich null sein
            Assert.That(ret != null);

            //ID muss übereinstimmen
            Assert.That(ret.match_id == id);

            //Lobby Type muss 7 sein
            Assert.That(ret.lobby_type == 7);

            //Game Mode muss 22 sein
            Assert.That(ret.game_mode == 22);

            //Game muss geparsed sein
            Assert.That(ret.version != -1);
        }

        [Test]
        public async Task FetchMatch_UngültigesMatch()
        {
            // Ungültiges Match
            Match ret = await _openDotaService.FetchMatch(6058018210);

            // Rückgabe muss null sein
            Assert.That(ret == null);
        }

        [Test]
        public async Task FetchMatch_UngültigeId()
        {
            // Nicht existierende MatchId
            Match ret = await _openDotaService.FetchMatch(1234567890);

            // Rückgabe muss null sein
            Assert.That(ret == null);
        }

        [Test]
        public async Task GetSteamIdByPersonaName_RealerSpieler()
        {
            string personaName = "iTz_Proph3t";
            long steam32Id = 114297134;

            long foundId = await _openDotaService.GetSteamIdByPersonaName(personaName);

            // Gefundene Steam32 Id stimmt mit korrekter Überein
            Assert.That(foundId == steam32Id);
        }

        [Test]
        public async Task GetSteamIdByPersonaName_Leerstring()
        {
            string personaName = "";

            long foundId = await _openDotaService.GetSteamIdByPersonaName(personaName);

            // Bei fehlerhafter Abfrage => Rückgabe = -1
            Assert.That(foundId == -1);
        }

        [Test]
        public async Task GetSteamIdByPersonaName_FiktiverName()
        {
            string personaName = "othaw_bdcc";

            long foundId = await _openDotaService.GetSteamIdByPersonaName(personaName);

            // gesuchter Name != gefundener Account => Rückgabe = -1
            Assert.That(foundId == -1);
        }
    }
}