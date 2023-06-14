using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Backend.Domain.Models;
using Backend.Domain.Services;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace Backend.API.Controllers
{
    // Controller für alle Anfragen an die OpenDota API

    [ApiController]
    [Route("api/[controller]")]
    public class OpenDotaController : ControllerBase
    {
        private readonly IOpenDotaService _openDotaService;

        public OpenDotaController(IOpenDotaService openDotaService)
        {
            _openDotaService = openDotaService;
        }

        /// <summary>
        /// Holt neue Matches bis vorgegebene Anzahl erreicht ist.
        /// </summary>
        /// <param name="number">Mindestanzahl neuer Matches</param>
        /// <param name="parse">Parse Anforderung für jedes Match</param>
        /// <param name="numberIsTarget">Erstellt maximal number neue Matches</param>
        /// <returns>Liste neuer Match Ids</returns>
        [HttpGet]
        [Route("fetchnewmatches")]
        public async Task<List<long>> FetchNewMatches(int number = 1, bool parse = true, bool numberIsTarget = false)

        {
            return await _openDotaService.FetchNewMatches(number, parse, numberIsTarget);
        }

        /// <summary>
        /// Holt neue Matches eines Spielers
        /// </summary>
        /// <param name="steam32id">Steam32 ID des Spielers</param>
        /// <param name="limit">Maximale Anzahl an zu prüfenden Spiele</param>
        /// <returns>Liste neuer Match Ids</returns>
        [HttpGet]
        [Route("fetchplayermatches")]
        public async Task<List<long>> FetchAllMatchesForPlayer(long steam32id, int limit = 10)
        {
            return await _openDotaService.FetchAllMatchesForPlayer(steam32id, limit);
        }

        /// <summary>
        /// Holt neueste Matches eines Spielers
        /// </summary>
        /// <param name="steam32id">Steam32 ID des Spielers</param>
        /// <returns>Liste neuer Match Ids</returns>
        [HttpGet]
        [Route("fetchrecentplayermatches")]
        public async Task<List<long>> FetchRecentMatchesForPlayer(long steam32id)
        {
            return await _openDotaService.FetchRecentMatchesForPlayer(steam32id);
        }

        /// <summary>
        /// Holt ein einzelnes Match anhand der Match ID. Wird optional vorher geparsed.
        /// </summary>
        /// <param name="matchId">ID des zu holenden Matches</param>
        /// <returns>Gesamtes Match falls Match gütlig, sonst null</returns>
        [HttpGet]
        [Route("fetchmatch")]
        public async Task<Match> FetchMatch(long matchId)
        {
            return await _openDotaService.FetchMatch(matchId);
        }

        /// <summary>
        /// Findet die Steam32Id zu einem Username
        /// </summary>
        /// <param name="name">Username des Spielers</param>
        /// <returns>Gefundene Steam32Id, sonst -1</returns>
        [HttpGet]
        [Route("getsteamid")]
        public async Task<long> GetSteamIdByPersonaName(string name)
        {
            return await _openDotaService.GetSteamIdByPersonaName(name);
        }
    }
}