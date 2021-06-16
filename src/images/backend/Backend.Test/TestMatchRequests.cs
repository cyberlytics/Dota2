using NUnit.Framework;
using Backend.Domain.Services;
using Backend.Domain.Models;


namespace Backend.Test
{
    [TestFixture]
    public class TestMatchRequests
    {
        private MatchRepository _matchRepository;
        private MatchstoreDatabaseSettings _matchstoreDatabaseSettings;

        // Hier fuer Tests notwendige Vorbereitungen ausfuehren,
        // falls notwendig
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

        // Einzelner Test
        [Test, Order(1)]
        public void RequestSingleMatch()
        {
            Match match = _matchRepository.Get(6044635209);

            // Was soll bei nicht existierender Match-ID zurueckgegeben werden?
            Assert.That(match != null);
            
            // Assert-Statements, die erfuellt sein muessen, damit der Test erfolgreich ist
            Assert.That(match.match_id == 6044635209);
        }
        
        
        [Test, Order(2)]
        public void RequestSingleMatchNotExisting()
        {
            // Match mit nicht existierender ID anfragen
            Match match = _matchRepository.Get(9999999);

            // Was soll bei nicht existierender Match-ID zurueckgegeben werden?
            Assert.That(match == null);
        }
    }
}