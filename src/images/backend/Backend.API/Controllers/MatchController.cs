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
    public class MatchController : ControllerBase
    {
        private readonly IMatchService _matchService;

        public MatchController(IMatchService matchService)
        {
            _matchService = matchService;
        }

        [HttpGet]
        [Route("test1 get all as MatchDto")]
        public ActionResult<List<MatchDto>> Test()
        {
            return _matchService.Get().ConvertAll(x => new MatchDto(x));
        }

        [HttpPost]
        [Route("test2 create new match")]
        public Match CreateMatch()
        {
            var match = new Match();
            match.match_id = DateTime.UtcNow.Ticks;
            match.chat = new List<Match.Chat>();
            var chatElement = new Match.Chat();
            chatElement.key = "gg";
            match.chat.Add(chatElement);
            match.chat.Add(chatElement);
            match.chat.Add(chatElement);

            return _matchService.Create(match);
        }

        [HttpGet]
        [Route("test3 get match by match_id")]
        public Match Get(long id)
        {
            return _matchService.Get(id);
        }

        [HttpGet]
        [Route("test4 get all available Matches as Matches")]
        public ActionResult<List<Match>> GetMatches()
        {
            return _matchService.Get();
        }

        [HttpDelete]
        [Route("test5 Deletes all matches")]
        public void DeleteAll()
        {
            foreach (Match m in _matchService.Get())
            {
                _matchService.Remove(m);
            }
        }
    }
}