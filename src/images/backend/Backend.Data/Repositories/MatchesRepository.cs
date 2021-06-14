using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection.Metadata.Ecma335;
using System.Threading.Tasks;
using Backend.Domain.Models;
using Backend.Domain.Repositories;
using Newtonsoft.Json;

namespace Backend.Data.Repositories
{
    public class MatchesRepository : IMatchesRepository
    {

        private List<Match> repository;
        private static string filePath = @"../Backend.Data/Database/matches.json";
        
        public MatchesRepository()
        {
            OpenRepository();
        }
        
        public void AddMatch(Match match)
        {
            if (this.repository.Find(x => x.match_id == match.match_id) == null)
            {
                this.repository.Add(match);
                SaveRepository();
            }
        }

        public Match FindMatch(long id)
        {
            throw new NotImplementedException();
        }

        public List<Match> RequestMatches(int startId, int cnt)
        {
            throw new NotImplementedException();
        }

        public List<Match> FindUserMatches(long userId)
        {
            throw new NotImplementedException();
        }

        public int MaxId()
        {
            throw new NotImplementedException();
        }

        private void OpenRepository()
        {
            if (File.Exists(filePath))
            {
                try
                {
                    using (StreamReader file = File.OpenText(filePath))
                    {
                        JsonSerializer serializer = new JsonSerializer();
                        this.repository = (List<Match>) serializer.Deserialize(file, typeof(List<Match>));
                    }
                }
                catch (Exception e)
                {
                    Console.Write($"Error while reading Match Repository: {e}");
                }
            }
            else
            {
                repository = new List<Match>();
            }
        }

        private void SaveRepository()
        {
            try
            {
                using (StreamWriter file = File.CreateText(filePath))
                {
                    JsonSerializer serializer = new JsonSerializer();
                    serializer.Serialize(file, repository);
                }
            }
            catch (Exception e)
            {
                Console.Write($"Error while writing Match Repository: {e}");
            }
        }
    }
}