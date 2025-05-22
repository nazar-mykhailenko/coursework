using AgroPlaner.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AgroPlaner.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class WeatherController : ControllerBase
    {
        private readonly IWeatherDataService _weatherDataService;

        public WeatherController(IWeatherDataService weatherDataService)
        {
            _weatherDataService = weatherDataService;
        }

        // POST: api/Weather/update-all
        [HttpPost("update-all")]
        // [Authorize(Roles = "Admin")] // Only admins can trigger weather updates for all locations
        public async Task<IActionResult> UpdateAllLocationsWeather()
        {
            await _weatherDataService.UpdateWeatherForAllLocationsAsync();
            return Ok(new { message = "Weather data update has been initiated for all locations" });
        }
    }
}
