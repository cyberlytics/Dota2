using System;
using System.Net;
using System.Threading.Tasks;
using Backend.Domain.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Backend.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class JupyterController : ControllerBase
    {
        private readonly IJupyterService _jupyterService;

        public JupyterController(IJupyterService jupyterService)
        {
            _jupyterService = jupyterService;
        }
        
        /// <summary>
        /// Frägt ein Match anhand der MatchID aus der Datenbank ab und schickt dieses zum Jupyter Server.
        /// </summary>
        /// <param name="matchId">ID des zu schreibenden Matches</param>
        /// <returns>StatusCode des Jupyter Servers</returns>
        [HttpGet]
        [Route("writematch")]
        public async Task<IActionResult> WriteMatch(long matchId)
        {
            var result = await _jupyterService.WriteMatchAsync(matchId);
            
            //Gibt den StatusCodes der Anfrage zum Python Backend zurück. Muss als int gecastet werden, damit ein StatusCode Objekt erzeugt werden kann.
            return StatusCode((int)result);
        }
        
        /// <summary>
        /// Löscht ein Match anhand der MatchId aus der Datenbank des Jupyter Servers.
        /// </summary>
        /// <param name="matchId">ID des zu löschenden Matches</param>
        /// <returns>StatusCode des Jupyter Servers</returns>
        [HttpGet]
        [Route("deletematch")]
        public async Task<IActionResult> DeleteMatch(long matchId)
        {
            var result = await _jupyterService.DeleteMatchAsync(matchId);
            
            //Gibt den StatusCodes der Anfrage zum Python Backend zurück. Muss als int gecastet werden, damit ein StatusCode Objekt erzeugt werden kann.
            return StatusCode((int)result);
        }

        /// <summary>
        /// Startet das Trainieren des Models auf dem Jupyter Server.
        /// </summary>
        /// <param name="model_name">Welches Model verwendet werden soll. "no_kda" oder "kda"</param>
        /// <returns>StatusCode des Jupyter Servers</returns>
        [HttpGet]
        [Route("trainmodel")]
        public async Task<IActionResult> TrainModel(string model_name = "kda")
        {
            var result = await _jupyterService.TrainModelAsync(model_name);
            
            //Gibt den StatusCodes der Anfrage zum Python Backend zurück. Muss als int gecastet werden, damit ein StatusCode Objekt erzeugt werden kann.
            return StatusCode((int)result);
        }
        
        /// <summary>
        /// Startet die Vorhersage des Models auf dem Jupyter Server.
        /// </summary>
        /// <param name="matchId">ID des zu vorhersagenden Matches.</param>
        /// <param name="model_name">Welches Model verwendet werden soll. "no_kda" oder "kda"</param>
        /// <returns></returns>
        [HttpGet]
        [Route("predict")]
        public async Task<IActionResult> Predict(long matchId, string model_name = "kda")
        {
            var result = await _jupyterService.PredictAsync(matchId, model_name);

            if (result == null) return StatusCode(StatusCodes.Status500InternalServerError);
            
            return Ok(result);
        }
    }
}