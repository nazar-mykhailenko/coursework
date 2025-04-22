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
    public class SoilDataController : ControllerBase
    {
        private readonly ISoilDataService _soilDataService;

        public SoilDataController(ISoilDataService soilDataService)
        {
            _soilDataService = soilDataService;
        }

        // GET: api/SoilData/crop/5
        [HttpGet("crop/{cropId}")]
        public async Task<ActionResult<SoilData>> GetSoilDataByCrop(int cropId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var soilData = await _soilDataService.GetByCropIdAsync(cropId, userId);

            if (soilData == null)
            {
                return NotFound();
            }

            return Ok(soilData);
        }

        // PUT: api/SoilData
        [HttpPut]
        public async Task<ActionResult<SoilData>> UpdateSoilData(SoilData soilData)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            try
            {
                var updatedSoilData = await _soilDataService.UpdateAsync(soilData, userId);
                return Ok(updatedSoilData);
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

        // POST: api/SoilData/crop/5/irrigation
        [HttpPost("crop/{cropId}/irrigation")]
        public async Task<ActionResult<SoilData>> ApplyIrrigation(
            int cropId,
            IrrigationEvent irrigationEvent
        )
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            try
            {
                var updatedSoilData = await _soilDataService.ApplyIrrigationEventAsync(
                    cropId,
                    irrigationEvent,
                    userId
                );
                return Ok(updatedSoilData);
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

        // POST: api/SoilData/crop/5/fertilization
        [HttpPost("crop/{cropId}/fertilization")]
        public async Task<ActionResult<SoilData>> ApplyFertilization(
            int cropId,
            FertilizationEvent fertilizationEvent
        )
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            try
            {
                var updatedSoilData = await _soilDataService.ApplyFertilizationEventAsync(
                    cropId,
                    fertilizationEvent,
                    userId
                );
                return Ok(updatedSoilData);
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
    }
}
