using System.Security.Claims;
using AgroPlaner.Api.Models;
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

        public LocationsController(ILocationService locationService)
        {
            _locationService = locationService;
        }

        // GET: api/Locations
        [HttpGet]
        public async Task<ActionResult<IEnumerable<LocationDto>>> GetLocations()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var locations = await _locationService.GetAllAsync(userId);
            var dtos = locations.Select(MapLocationToDto);
            return Ok(dtos);
        }

        // GET: api/Locations/5
        [HttpGet("{id}")]
        public async Task<ActionResult<LocationDto>> GetLocation(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var location = await _locationService.GetByIdAsync(id, userId);

            if (location == null)
            {
                return NotFound();
            }

            return Ok(MapLocationToDto(location));
        }

        // POST: api/Locations
        [HttpPost]
        public async Task<ActionResult<LocationDto>> CreateLocation(CreateLocationDto createDto)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            try
            {
                var location = new Location
                {
                    Name = createDto.Name,
                    Latitude = createDto.Latitude,
                    Longitude = createDto.Longitude,
                };

                var createdLocation = await _locationService.CreateAsync(location, userId);
                var dto = MapLocationToDto(createdLocation);

                return CreatedAtAction(nameof(GetLocation), new { id = dto.LocationId }, dto);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // PUT: api/Locations/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateLocation(int id, UpdateLocationDto updateDto)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            try
            {
                var location = await _locationService.GetByIdAsync(id, userId);
                if (location == null)
                {
                    return NotFound();
                }

                // Update properties
                location.Name = updateDto.Name;
                location.Latitude = updateDto.Latitude;
                location.Longitude = updateDto.Longitude;

                var updatedLocation = await _locationService.UpdateAsync(location, userId);
                return Ok(MapLocationToDto(updatedLocation));
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

        // Helper method to map Location domain model to LocationDto
        private static LocationDto MapLocationToDto(Location location)
        {
            return new LocationDto
            {
                LocationId = location.LocationId,
                Name = location.Name,
                Latitude = location.Latitude,
                Longitude = location.Longitude,
            };
        }
    }
}
