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
        
        // Sicher in der Datenbank hinterlegte Match-ID
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

        /// <summary>
        /// Test, ob das Abrufen aller Matches aus der DB korrekt funktioniert
        /// </summary>
        [Test]
        public void Get_Alle_Matches_Abrufen()
        {
            List<Match> matches = _matchRepository.Get();

            Assert.That(matches.Count >= 200);
        }

        /// <summary>
        /// Test, ob das Abrufen eines konkreten Matches per ID korrekt funktioniert
        /// </summary>
        [Test]
        public void Get_Vorhandenes_Match_Abrufen()
        {
            Match match = _matchRepository.Get(existingId);

            Assert.That(match != null);
        }

        /// <summary>
        /// Test, ob das Abrufen eines konkreten Matches per ID, das nicht existiert, korrekt funktioniert
        /// </summary>
        [Test]
        public void Get_Nicht_Vorhandenes_Match_Abrufen()
        {
            Match match = _matchRepository.Get(1234567890);

            Assert.That(match == null);
        }

        /// <summary>
        /// Test, ob das Schreiben und Loeschen eines Matches auf der DB korrekt funktioniert
        /// </summary>
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

        /// <summary>
        /// Test, ob das Aktualisieren eines Matchs ueber seine ID korrekt funktioniert
        /// </summary>
        [Test]
        public void Create_Match()
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
        
        /// <summary>
        /// Test, ob das Entfernen eines konkreten Matches per ID korrekt funktioniert
        /// </summary>
        [Test]
        public void Remove_Vorhandenes_Match_Id ()
        {
            // Gespeichertes Match
            Match match = _matchRepository.Get(existingId);
            
            // Anzahl Vorkommnisse existierendes Match abrufen
            long matchCount = _matchRepository.GetCount(existingId);

            // Match ueber ID entfernen
            _matchRepository.Remove(existingId);

            // Match muss einmal weniger vorhanden sein
            Assert.That(matchCount == _matchRepository.GetCount(existingId) - 1);

            // Match wieder hinzufuegen
            _matchRepository.Create(match);
        }

        /// <summary>
        /// Test, ob das Entfernen eines konkreten Matches per Match-Objekt korrekt funktioniert
        /// </summary>
        [Test]
        public void Remove_Vorhandenes_Match()
        {
            // Existierendes Match abrufen
            Match match = _matchRepository.Get(existingId);
            
            // Anzahl Vorkommnisse existierendes Match abrufen
            long matchCount = _matchRepository.GetCount(existingId);

            // Match ueber ID entfernen
            _matchRepository.Remove(match);

            // Match muss einmal weniger vorhanden sein
            Assert.That(matchCount == _matchRepository.GetCount(existingId) - 1);

            // Match wieder hinzufuegen
            _matchRepository.Create(match);
        }

        /// <summary>
        /// Test, ob das Entfernen eines konkreten Matches per ID, das nicht existiert, korrekt funktioniert
        /// </summary>
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

        /// <summary>
        /// Test, ob das Entfernen eines konkreten Matches per Match-Objekt, das nicht existiert, korrekt funktioniert
        /// </summary>
        [Test]
        public void Remove_Fehlendes_Match()
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