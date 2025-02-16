using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DLW.BFF.Template.API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class WeatherForecastController : ControllerBase
    {

        [HttpGet(Name = "GetWeatherForecast")]
        public IEnumerable<string> Get()
        {
            string[] summaries = ["Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"];
            return Enumerable
                .Range(1, 5)
                .Select(index => summaries[Random.Shared.Next(summaries.Length)])
                .ToArray();
        }
    }
}
