using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Backend.Domain.Models;
using Backend.Domain.Services;
using Microsoft.AspNetCore.Mvc;

namespace Backend.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MatchesController : ControllerBase
    {
        private readonly IMatchesService _matchesService;

        public MatchesController(IMatchesService matchesService)
        {
            _matchesService = matchesService;
        }

        [HttpGet]
        [Route("single")]
        public async Task<MatchDto> GetMatch(long id)
        {
            return _matchesService.FindMatch(id);
        }
        
        [HttpGet]
        [Route("list")]
        public async Task<List<MatchDto>> GetMatches(int startId, int cnt)
        {
            return _matchesService.RequestMatches(startId, cnt);
        }
    }
}