using NUnit.Framework;
using Backend.Domain.Services;
using Backend.Domain.Models;


namespace Backend.Test
{
    [TestFixture]
    public class MatchDtoTest
    {
        private MatchRepository _matchRepository;
        private MatchstoreDatabaseSettings _matchstoreDatabaseSettings;

        // Sicher in der DB hinterlegte Match-ID
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
        }

        /// <summary>
        /// Test, ob ein neues MatchDto-Objekt korrekt erstellt wird
        /// </summary>
        [Test]
        public void MatchDto_Erstellen()
        {
            // MatchDto aus Match erstellen
            MatchDto matchDto = new MatchDto(_matchRepository.Get(existingId));

            // Match-ID muss uebereinstimmen
            Assert.That(matchDto.match_id == existingId);
        }

        /// <summary>
        /// Test, ob die MatchDtoPlayer-Objekte korrekt erstellt werden
        /// </summary>
        [Test]
        public void MatchDto_Erstellen_MatchDtoPlayer_Abrufen()
        {
            // MatchDto aus Match erstellen
            MatchDto matchDto = new MatchDto(_matchRepository.Get(existingId));
            
            // Spielerzahl muss 10 betragen
            Assert.That(matchDto.players.Count == 10);

            // Jeder Spieler muss Ping-Attribut besitzen
            foreach (var player in matchDto.players)
            {
                Assert.That(player.pings >= 0);
            }
        }
    }
}