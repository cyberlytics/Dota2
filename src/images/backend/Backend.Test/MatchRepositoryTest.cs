using NUnit.Framework;
using Backend.Domain.Services;
using Backend.Domain.Models;
using System.Collections.Generic;

namespace Backend.Test
{
    [TestFixture]
    public class MatchRepositoryTest
    {
        private MatchRepository _matchRepository;
        private MatchstoreDatabaseSettings _matchstoreDatabaseSettings;

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
        }

        [Test]
        public void Get_Alle_Matches_Abrufen()
        {
            List<Match> matches = _matchRepository.Get();

            Assert.That(matches.Count == 200);
        }

        [Test]
        public void Get_Vorhandenes_Match_Abrufen()
        {
            Match match = _matchRepository.Get(6049286410);

            Assert.That(match != null);
        }

        [Test]
        public void Get_Nicht_Vorhandenes_Match_Abrufen()
        {
            Match match = _matchRepository.Get(1234567890);

            Assert.That(match == null);
        }

        [Test]
        public void Create_Neues_Match_In_Datenbank_Schreiben_und_Entfernen()
        {
            // Anzahl muss wieder Default-Datensatz sein
            List<Match> matches = _matchRepository.Get();
            int defaultCount = matches.Count;
            Match createMatch = new Match() { match_id = 1 };

            // Match hinzufügen
            Match createdMatch = _matchRepository.Create(createMatch);

            Assert.That(createdMatch.match_id == 1);
            Assert.That(_matchRepository.Get().Count == defaultCount + 1);

            // Match entfernen
            _matchRepository.Remove(1);

            // Anzahl muss wieder der Anzahl am Anfang entsprechen
            Assert.That(_matchRepository.Get().Count == defaultCount);
        }

        [Test]
        public void Create_Match_Aktualisieren()
        {
            // Anzahl muss wieder Default-Datensatz sein
            List<Match> matches = _matchRepository.Get();
            int defaultCount = matches.Count;
            Match createMatch = new Match() { match_id = 1 };

            // Match hinzufügen
            Match createdMatch = _matchRepository.Create(createMatch);

            Assert.That(createdMatch.match_id == 1);
            Assert.That(_matchRepository.Get().Count == defaultCount + 1);

            // Match aktualisieren
            createMatch.match_id = 2;
            _matchRepository.Update(1, createMatch);
            Match updatedMatch = _matchRepository.Get(2);

            // Aktualisierung überprüfen
            Assert.That(updatedMatch.match_id == 2);

            // Match entfernen
            _matchRepository.Remove(2);

            // Anzahl muss wieder der Anzahl am Anfang entsprechen
            Assert.That(_matchRepository.Get().Count == defaultCount);
        }
    }
}