using System.Security.Claims;
using AgroPlaner.Api.Models;
using AgroPlaner.DAL.Models;
using AgroPlaner.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AgroPlaner.Api.Controllers
{
    // Record to hold crops and their total count
    public record CropsResult(IEnumerable<CropDto> Items, int TotalCount);

    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class CropsController : ControllerBase
    {
        private readonly ICropService _cropService;

        public CropsController(ICropService cropService)
        {
            _cropService = cropService;
        }

        // GET: api/Crops
        [HttpGet]
        public async Task<ActionResult<CropsResult>> GetCrops(
            [FromQuery] int? pageNumber = null,
            [FromQuery] int? pageSize = null
        )
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (pageNumber.HasValue && pageSize.HasValue)
            {
                var result = await _cropService.GetPagedAsync(
                    pageNumber.Value,
                    pageSize.Value,
                    userId
                );
                var dtos = result.Items.Select(MapCropToDto);
                return Ok(new CropsResult(dtos, result.TotalCount));
            }
            else
            {
                var crops = await _cropService.GetAllAsync(userId);
                var dtos = crops.Select(MapCropToDto);
                return Ok(new CropsResult(dtos, crops.Count()));
            }
        }

        // GET: api/Crops/5
        [HttpGet("{id}")]
        public async Task<ActionResult<CropDto>> GetCrop(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var crop = await _cropService.GetByIdAsync(id, userId);

            if (crop == null)
            {
                return NotFound();
            }

            return Ok(MapCropToDto(crop));
        }

        // POST: api/Crops
        [HttpPost]
        public async Task<ActionResult<CropDto>> CreateCrop(CreateCropDto createDto)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized(new { message = "User ID not found in token" });
            }

            try
            {
                var cropToCreate = new Crop
                {
                    Name = createDto.Name,
                    PlantId = createDto.PlantId,
                    LocationId = createDto.LocationId,
                    ExpectedYield = createDto.ExpectedYield,
                    FieldArea = createDto.FieldArea,
                    CumulativeGDDToday =
                        0 // Default value for new crops
                    ,
                };

                var createdCrop = await _cropService.CreateAsync(cropToCreate, userId);
                var dto = MapCropToDto(createdCrop);

                return CreatedAtAction(nameof(GetCrop), new { id = dto.Id }, dto);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // PUT: api/Crops/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCrop(int id, UpdateCropDto updateDto)
        {
            if (id != updateDto.Id)
            {
                return BadRequest("The ID in the URL does not match the ID in the request body.");
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized(new { message = "User ID not found in token" });
            }

            try
            {
                // First, get the existing crop
                var existingCrop = await _cropService.GetByIdAsync(id, userId);
                if (existingCrop == null)
                {
                    return NotFound();
                }

                // Update only the properties that can be modified
                existingCrop.Name = updateDto.Name;
                existingCrop.ExpectedYield = updateDto.ExpectedYield;
                existingCrop.FieldArea = updateDto.FieldArea;

                var updatedCrop = await _cropService.UpdateAsync(existingCrop, userId);
                return Ok(MapCropToDto(updatedCrop));
            }
            catch (UnauthorizedAccessException)
            {
                return Forbid();
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }        // DELETE: api/Crops/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCrop(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized(new { message = "User ID not found in token" });
            }

            var result = await _cropService.DeleteAsync(id, userId);

            if (!result)
            {
                return NotFound();
            }

            return NoContent();
        }

        // Helper method to map Crop domain model to CropDto
        private static CropDto MapCropToDto(Crop crop)
        {
            return new CropDto
            {
                Id = crop.Id,
                Name = crop.Name,
                PlantId = crop.PlantId,
                LocationId = crop.LocationId,
                ExpectedYield = crop.ExpectedYield,
                CumulativeGDDToday = crop.CumulativeGDDToday,
                FieldArea = crop.FieldArea,
            };
        }
    }
}
