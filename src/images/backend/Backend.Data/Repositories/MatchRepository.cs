using Backend.Domain.Models;
using Backend.Domain.Repositories;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Backend.Domain.Services
{
    public class MatchRepository : IMatchRepository
    {
        private readonly IMongoCollection<Match> _matches;

        public MatchRepository(IMatchstoreDatabaseSettings settings)
        {
            string connectionString = Environment.GetEnvironmentVariable("MONGODB_CONNECTION_STRING") != null ?
                Environment.GetEnvironmentVariable("MONGODB_CONNECTION_STRING") : settings.ConnectionString;
            var client = new MongoClient(connectionString);
            var database = client.GetDatabase(settings.DatabaseName);

            _matches = database.GetCollection<Match>(settings.MatchesCollectionName);
        }

        List<Match> IMatchRepository.Get()
        {
            return _matches.Find(m => true).ToList();
        }

        Match IMatchRepository.Get(long id)
        {
            return _matches.Find<Match>(m => m.match_id == id).FirstOrDefault();
        }

        public Match Create(Match match)
        {
            _matches.InsertOne(match);
            return match;
        }

        public void Update(long id, Match match)
        {
            _matches.ReplaceOne(m => m.match_id == id, match);
        }

        public void Remove(Match match)
        {
            _matches.DeleteOne(m => m.match_id == match.match_id);
        }

        public void Remove(long id)
        {
            _matches.DeleteOne(m => m.match_id == id);
        }
    }
}
