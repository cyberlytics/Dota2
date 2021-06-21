using System;
using System.Collections.Generic;
using NUnit.Framework;
using Backend.Domain.Services;
using Backend.Domain.Models;
using System.Collections.Generic;
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

        private readonly long existingId = 6049286410;

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
        public async Task FetchNewMatches_Matches_Abrufen()
        {
            List<Match> matches = _matchRepository.Get();

            int currentMatchCount = matches.Count;

            await _openDotaService.FetchNewMatches(1);

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
        public async Task FetchNewMatchesAndParse_Matches_Abrufen_Parsen()
        {
            int newMatchCount = 1;
            
            int currentMatchCount = _matchRepository.Get().Count;

            await _openDotaService.FetchNewMatchesAndParse(newMatchCount);
            
            // Zahl der Matches mindestens um die angeforderte Zahl erhöht
            Assert.That(_matchRepository.Get().Count >= currentMatchCount + newMatchCount);
        }
        
        [Test]
        public async Task FetchNewMatchesAndParse_Null_Abrufen()
        {
            int matchCount = _matchRepository.Get().Count;

            await _openDotaService.FetchNewMatchesAndParse(0);

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
        public async Task FetchAllMatchesForPlayer_Null_Abrufen()
        {
            int matchCount = _matchRepository.Get().Count;

            await _openDotaService.FetchAllMatchesForPlayer(114297134, 0);

            // Matchzahl muss aufgrund Limit 0 gleich bleiben
            Assert.That(_matchRepository.Get().Count == matchCount);
        }

    }
}