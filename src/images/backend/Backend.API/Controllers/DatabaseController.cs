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

        public DatabaseController(IMatchRepository matchRepository, IJupyterService jupyterService)
        {
            _matchRepository = matchRepository;
            _jupyterService = jupyterService;
        }
        
        [HttpGet]
        [Route("getAll")]
        public Match GetAll()
        {
            return _matchRepository.Get().FirstOrDefault();
        }
        
        [HttpGet]
        [Route("get")]
        public Match Get(long id)
        {
            return _matchRepository.Get(id);
        }
        
        [HttpGet]
        [Route("move")]
        public void MoveAll()
        {
            var list = _matchRepository.Get();

            foreach (var match in list)
            {
                _jupyterService.WriteMatchAsync(match.match_id);
            }
        }
    }
}