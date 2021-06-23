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
        private long existingId = 6049286410;


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

            Assert.That(matches.Count >= 200);
        }

        [Test]
        public void Get_Vorhandenes_Match_Abrufen()
        {
            Match match = _matchRepository.Get(existingId);

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

            // Match hinzuf�gen
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

            // Match hinzuf�gen
            Match createdMatch = _matchRepository.Create(createMatch);

            Assert.That(createdMatch.match_id == 1);
            Assert.That(_matchRepository.Get().Count == defaultCount + 1);

            // Match aktualisieren
            createMatch.match_id = 2;
            _matchRepository.Update(1, createMatch);
            Match updatedMatch = _matchRepository.Get(2);

            // Aktualisierung �berpr�fen
            Assert.That(updatedMatch.match_id == 2);

            // Match entfernen
            _matchRepository.Remove(2);

            // Anzahl muss wieder der Anzahl am Anfang entsprechen
            Assert.That(_matchRepository.Get().Count == defaultCount);
        }

        [Test]
        public void Remove_Vorhandenes_Match_Entfernen_Id ()
        {
            // Existierendes Match abrufen
            Match match = _matchRepository.Get(existingId);

            // Match ueber ID entfernen
            _matchRepository.Remove(existingId);

            // Match darf nicht mehr vorhanden sein
            Assert.That(_matchRepository.Get(existingId) == null);

            // Match wieder hinzufuegen
            _matchRepository.Create(match);
        }

        [Test]
        public void Remove_Vorhandenes_Match_Entfernen_Match()
        {
            // Existierendes Match abrufen
            Match match = _matchRepository.Get(existingId);

            // Match ueber ID entfernen
            _matchRepository.Remove(match);

            // Match darf nicht mehr vorhanden sein
            Assert.That(_matchRepository.Get(existingId) == null);

            // Match wieder hinzufuegen
            _matchRepository.Create(match);
        }

        [Test]
        public void Remove_Fehlendes_Match_Entfernen_ID()
        {
            // Nicht vorhandene Match-ID
            long randomId = 123456;

            // Match mit nicht existierender ID entfernen
            _matchRepository.Remove(randomId);

            // Match darf nicht hinterlegt sein
            Assert.That(_matchRepository.Get(randomId) == null);
        }

        [Test]
        public void Remove_Fehlendes_Match_Entfernen_Match()
        {
            // Nicht vorhandene Match-ID
            long randomId = 123456;

            // Matchobjekt mit nicht vorhandener ID erstellen
            Match match = new Match
            {
                match_id = randomId
            };

            // Match mit nicht existierender ID entfernen
            _matchRepository.Remove(match);

            // Match darf nicht hinterlegt sein
            Assert.That(_matchRepository.Get(randomId) == null);
        }
    }
}