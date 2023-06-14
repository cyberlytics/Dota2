using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Backend.Domain.Models;
using Backend.Domain.Services;
using Microsoft.AspNetCore.Mvc;

namespace Backend.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DatabaseController : ControllerBase
    {
        private readonly IMatchRepository _matchRepository;
        private readonly IJupyterService _jupyterService;
        
        /// <summary>
        /// Controller zum Testen der Datenbank.
        /// </summary>
        /// <param name="matchRepository"></param>
        /// <param name="jupyterService"></param>
        public DatabaseController(IMatchRepository matchRepository, IJupyterService jupyterService)
        {
            _matchRepository = matchRepository;
            _jupyterService = jupyterService;
        }
        
        /// <summary>
        /// Gibt alle in der Matchverwaltungs-DB gespeicherten Matches zurueck
        /// </summary>
        /// <returns>Alle gespeicherten Matches</returns>
        [HttpGet]
        [Route("getAll")]
        public List<Match> GetAll()
        {
            return _matchRepository.Get();
        }
        
        /// <summary>
        /// Gibt ein in der Matchverwaltungs-DB gespeichertes Match mit bestimmter ID zurueck, falls dieses existiert
        /// </summary>
        /// <param name="id">Match-ID des gesuchten Matchs</param>
        /// <returns>Match, falls gefunden, sonst null</returns>
        [HttpGet]
        [Route("get")]
        public Match Get(long id)
        {
            return _matchRepository.Get(id);
        }
        
        /// <summary>
        /// Schreibt alle in der Matchverwaltungs-DB hinterlegten Matches in die Matchanalyse-DB
        /// </summary>
        /// <returns>void</returns>
        [HttpGet]
        [Route("move")]
        public void Move(long id)
        {
            _jupyterService.WriteMatchAsync(id);
        }
        
        [HttpGet]
        [Route("moveAll")]
        public void MoveAll()
        {
            // Alle Matches
            var list = _matchRepository.Get();

            foreach (var match in list)
            {
                // Match ueber Jupyter-Service in Matchanalyse-DB schreiben
                _jupyterService.WriteMatchAsync(match.match_id);
            }
        }
    }
}