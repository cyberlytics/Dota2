using Microsoft.AspNetCore.Mvc;

namespace Backend.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class GameDataController : ControllerBase
    {
        [HttpGet]
        public string GetGameData(int id)
        {
            return "ID: " + id;
        }
    }
}