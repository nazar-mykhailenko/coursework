using System.Security.Claims;
using AgroPlaner.DAL.Models;
using AgroPlaner.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AgroPlaner.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class LocationsController : ControllerBase
    {
        private readonly ILocationService _locationService;
        private readonly IWeatherDataService _weatherDataService;

        public LocationsController(
            ILocationService locationService,
            IWeatherDataService weatherDataService
        )
        {
            _locationService = locationService;
            _weatherDataService = weatherDataService;
        }

        // GET: api/Locations
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Location>>> GetLocations()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var locations = await _locationService.GetAllAsync(userId);
            return Ok(locations);
        }

        // GET: api/Locations/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Location>> GetLocation(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var location = await _locationService.GetByIdAsync(id, userId);

            if (location == null)
            {
                return NotFound();
            }

            return Ok(location);
        }

        // GET: api/Locations/5/weather
        [HttpGet("{id}/weather")]
        public async Task<ActionResult<WeatherData>> GetLocationWeather(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var location = await _locationService.GetByIdAsync(id, userId);

            if (location == null)
            {
                return NotFound();
            }

            var weatherData = await _weatherDataService.GetTodayWeatherForLocationAsync(id);

            if (weatherData == null)
            {
                return NotFound("Weather data not available for this location");
            }

            return Ok(weatherData);
        }

        // GET: api/Locations/5/weather/history
        [HttpGet("{id}/weather/history")]
        public async Task<ActionResult<IEnumerable<WeatherData>>> GetLocationWeatherHistory(
            int id,
            [FromQuery] DateTime startDate,
            [FromQuery] DateTime endDate
        )
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var location = await _locationService.GetByIdAsync(id, userId);

            if (location == null)
            {
                return NotFound();
            }

            var weatherHistory = await _weatherDataService.GetHistoryByLocationIdAsync(
                id,
                startDate,
                endDate
            );
            return Ok(weatherHistory);
        }

        // POST: api/Locations
        [HttpPost]
        public async Task<ActionResult<Location>> CreateLocation(Location location)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            try
            {
                var createdLocation = await _locationService.CreateAsync(location, userId);
                return CreatedAtAction(
                    nameof(GetLocation),
                    new { id = createdLocation.LocationId },
                    createdLocation
                );
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // PUT: api/Locations/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateLocation(int id, Location location)
        {
            if (id != location.LocationId)
            {
                return BadRequest("The ID in the URL does not match the ID in the request body.");
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            try
            {
                var updatedLocation = await _locationService.UpdateAsync(location, userId);
                return Ok(updatedLocation);
            }
            catch (UnauthorizedAccessException)
            {
                return Forbid();
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // DELETE: api/Locations/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteLocation(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var result = await _locationService.DeleteAsync(id, userId);

            if (!result)
            {
                return NotFound();
            }

            return NoContent();
        }
    }
}
