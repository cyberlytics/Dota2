using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Backend.Domain.Models;
using Backend.Domain.Services;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace Backend.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MatchesController
    {
        private readonly IMatchesService _matchesService;

        public MatchesController(IMatchesService matchesService)
        {
            _matchesService = matchesService;
        }
        
        [HttpGet]
        [Route("request")]
        public async Task<Match> RequestMatch(long id)
        {
            return await _matchesService.RequestMatch(id);
        }
        
        [HttpGet]
        [Route("all")]
        public List<Match> GetAllMatches()
        {
            return _matchesService.GetAllMatches();
        }
    }
}