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
    public class CropsController : ControllerBase
    {
        private readonly ICropService _cropService;

        public CropsController(ICropService cropService)
        {
            _cropService = cropService;
        }

        // GET: api/Crops
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Crop>>> GetCrops()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var crops = await _cropService.GetAllAsync(userId);
            return Ok(crops);
        }

        // GET: api/Crops/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Crop>> GetCrop(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var crop = await _cropService.GetByIdAsync(id, userId);

            if (crop == null)
            {
                return NotFound();
            }

            return Ok(crop);
        }

        // GET: api/Crops/paged?pageNumber=1&pageSize=10
        [HttpGet("paged")]
        public async Task<ActionResult<IEnumerable<Crop>>> GetPagedCrops(
            int pageNumber = 1,
            int pageSize = 10
        )
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var result = await _cropService.GetPagedAsync(pageNumber, pageSize, userId);

            Response.Headers.Add("X-Total-Count", result.TotalCount.ToString());
            return Ok(result.Items);
        }

        // POST: api/Crops
        [HttpPost]
        public async Task<ActionResult<Crop>> CreateCrop(Crop crop)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            try
            {
                var createdCrop = await _cropService.CreateAsync(crop, userId);
                return CreatedAtAction(nameof(GetCrop), new { id = createdCrop.Id }, createdCrop);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // PUT: api/Crops/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCrop(int id, Crop crop)
        {
            if (id != crop.Id)
            {
                return BadRequest("The ID in the URL does not match the ID in the request body.");
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            try
            {
                var updatedCrop = await _cropService.UpdateAsync(crop, userId);
                return Ok(updatedCrop);
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

        // DELETE: api/Crops/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCrop(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var result = await _cropService.DeleteAsync(id, userId);

            if (!result)
            {
                return NotFound();
            }

            return NoContent();
        }
    }
}
