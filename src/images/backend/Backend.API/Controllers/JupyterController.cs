using System.Net;
using System.Threading.Tasks;
using Backend.Domain.Services;
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
        
        [HttpGet]
        [Route("writematch")]
        public async Task<IActionResult> WriteMatch(long matchId)
        {
            var result = await _jupyterService.WriteMatchAsync(matchId);
            
            //Gibt den StatusCodes der Anfrage zum Python Backend zurück. Muss als int gecastet werden, damit ein StatusCode Objekt erzeugt werden kann.
            return StatusCode((int)result);
        }
    }
}