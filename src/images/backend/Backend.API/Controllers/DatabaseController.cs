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

        public DatabaseController(IMatchRepository matchRepository)
        {
            _matchRepository = matchRepository;
        }
        
        [HttpGet]
        [Route("get")]
        public Match GetAll()
        {
            return _matchRepository.Get().FirstOrDefault();
        }
    }
}