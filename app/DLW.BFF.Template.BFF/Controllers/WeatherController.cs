using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Abstractions;

namespace DLW.BFF.Template.BFF.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/[controller]")]
    public class WeatherController : ControllerBase
    {
        private readonly IDownstreamApi _downstreamApi;

        public WeatherController(IDownstreamApi downstreamApi)
        {
            _downstreamApi = downstreamApi;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var data = await _downstreamApi.CallApiForUserAsync<string[]>(
                "DownstreamAPI",
                options => { 
                    options.HttpMethod = "GET";
                    options.RelativePath = "api/WeatherForecast";
                }
            );
            return Ok(data);
        }
    }
}
