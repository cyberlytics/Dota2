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

        /// <summary>
        /// Gibt alle hinterlegten Matches in Listenform zurueck
        /// </summary>
        /// <returns>Liste mit allen Matches</returns>
        public List<Match> Get()
        {
            return _matches.Find(m => true).ToList();
        }

        /// <summary>
        /// Gibt Match mit bestimmter ID zurueck, falls dieses existiert
        /// </summary>
        /// <param name="id">Match-ID des gesuchten Matchs</param>
        /// <returns>Match, falls gefunden, sonst null</returns>
        public Match Get(long id)
        {
            return _matches.Find<Match>(m => m.match_id == id).FirstOrDefault();
        }

        /// <summary>
        /// Speichert ein Matchobjekt in der Datenbank
        /// </summary>
        /// <param name="match">Zu hinterlegendes Match</param>
        /// <returns>Erstelltes Match</returns>
        public Match Create(Match match)
        {
            _matches.InsertOne(match);
            return match;
        }

        /// <summary>
        /// Ersetzt ein gespeichertes Match
        /// </summary>
        /// <param name="id">Match-ID des zu ersetzenden Matchs</param>
        /// /// <param name="match">Match, das als Ersatz dient</param>
        public void Update(long id, Match match)
        {
            _matches.ReplaceOne(m => m.match_id == id, match);
        }

        /// <summary>
        /// Entfernt ein Match aus der Datenbank
        /// </summary>
        /// <param name="match">Zu entfernendes</param>
        public void Remove(Match match)
        {
            Remove(match.match_id);
        }

        /// <summary>
        /// Entfernt Match mit bestimmter ID aus der Datenbank
        /// </summary>
        /// <param name="id">ID des zu entfernenden Matchs</param>
        public void Remove(long id)
        {
            _matches.DeleteOne(m => m.match_id == id);
        }
    }
}
