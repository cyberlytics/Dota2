using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Backend.Domain.Models;
using Backend.Domain.Services;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace Backend.API.Controllers
{
    //Controller für alle Anfragen an die OpenDota API

    [ApiController]
    [Route("api/[controller]")]
    public class OpenDotaController : ControllerBase
    {
        private readonly IOpenDotaService _openDotaService;

        public OpenDotaController(IOpenDotaService openDotaService)
        {
            _openDotaService = openDotaService;
        }

        [HttpGet]
        [Route("fetch")]
        public async Task<string> FetchNewMatches(int number = 1)
        {
            return await _openDotaService.FetchNewMatches(number);
        }

        [HttpGet]
        [Route("parse")]
        public async Task<string> ParseNewMatches(int number = 1)
        {
            return await _openDotaService.FetchNewMatchesAndParse(number);
        }
    }
}