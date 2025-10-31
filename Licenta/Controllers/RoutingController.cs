using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System.Net.Http;
using System.Threading.Tasks;

namespace Licenta.Controllers
{
    [ApiController]
    [Route("api/routing")]
    public class RoutingController : ControllerBase
    {
        private readonly HttpClient _http;
        private readonly IConfiguration _config;

        public RoutingController(HttpClient http, IConfiguration config)
        {
            _http = http;
            _config = config;
        }

        [HttpGet("route")]
        public async Task<IActionResult> GetRoute([FromQuery] string start, [FromQuery] string end, [FromQuery] string mode = "driving-car")
        {
            string apiKey = _config["OpenRouteService:ApiKey"];
            string url = $"https://api.openrouteservice.org/v2/directions/{mode}?api_key={apiKey}&start={start}&end={end}";

            var response = await _http.GetAsync(url);
            var content = await response.Content.ReadAsStringAsync();
            return Content(content, "application/json");
        }
    }
}
