using Backend.Domain.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Backend.Domain.Models
{
    /// <summary>
    /// Modell der Einstellungen, die zu einer MongoDB-Verbindung verwendet werden.
    /// </summary>
    public class MatchstoreDatabaseSettings : IMatchstoreDatabaseSettings
    {
        public string MatchesCollectionName { get; set; }
        public string ConnectionString { get; set; }
        public string DatabaseName { get; set; }
    }
}
