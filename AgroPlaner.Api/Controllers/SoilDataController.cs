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
    public class SoilDataController : ControllerBase
    {
        private readonly ISoilDataService _soilDataService;

        public SoilDataController(ISoilDataService soilDataService)
        {
            _soilDataService = soilDataService;
        }

        // GET: api/SoilData/crop/5
        [HttpGet("crop/{cropId}")]
        public async Task<ActionResult<SoilDataDto>> GetSoilDataByCrop(int cropId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var soilData = await _soilDataService.GetByCropIdAsync(cropId, userId);

            if (soilData == null)
            {
                return NotFound();
            }

            return Ok(MapSoilDataToDto(soilData));
        }

        // PUT: api/SoilData
        [HttpPut]
        public async Task<ActionResult<SoilDataDto>> UpdateSoilData(SoilDataDto soilDataDto)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            try
            {
                // Convert DTO to domain model
                var soilData = new SoilData
                {
                    SoilDataId = soilDataDto.SoilDataId,
                    CropId = soilDataDto.CropId,
                    CurrentMoisture = soilDataDto.CurrentMoisture,
                    FieldCapacity = soilDataDto.FieldCapacity,
                    Temperature = soilDataDto.Temperature,
                    AvailableNitrogen = soilDataDto.AvailableNitrogen,
                    AvailablePhosphorus = soilDataDto.AvailablePhosphorus,
                    AvailablePotassium = soilDataDto.AvailablePotassium,
                };

                var updatedSoilData = await _soilDataService.UpdateAsync(soilData, userId);
                return Ok(MapSoilDataToDto(updatedSoilData));
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
        public async Task<ActionResult<SoilDataDto>> ApplyIrrigation(
            int cropId,
            IrrigationEventDto irrigationEventDto
        )
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            try
            {
                // Convert DTO to domain model
                var irrigationEvent = new IrrigationEvent
                {
                    Date = DateTime.Now,
                    Amount = irrigationEventDto.Amount,
                };

                var updatedSoilData = await _soilDataService.ApplyIrrigationEventAsync(
                    cropId,
                    irrigationEvent,
                    userId
                );
                return Ok(MapSoilDataToDto(updatedSoilData));
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
        public async Task<ActionResult<SoilDataDto>> ApplyFertilization(
            int cropId,
            FertilizationEventDto fertilizationEventDto
        )
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            try
            {
                // Convert DTO to domain model
                var fertilizationEvent = new FertilizationEvent
                {
                    Date = DateTime.Now,
                    NitrogenAmount = fertilizationEventDto.NitrogenAmount,
                    PhosphorusAmount = fertilizationEventDto.PhosphorusAmount,
                    PotassiumAmount = fertilizationEventDto.PotassiumAmount,
                };

                var updatedSoilData = await _soilDataService.ApplyFertilizationEventAsync(
                    cropId,
                    fertilizationEvent,
                    userId
                );
                return Ok(MapSoilDataToDto(updatedSoilData));
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

        // Helper method to map SoilData domain model to SoilDataDto
        private static SoilDataDto MapSoilDataToDto(SoilData soilData)
        {
            return new SoilDataDto
            {
                SoilDataId = soilData.SoilDataId,
                CropId = soilData.CropId,
                CurrentMoisture = soilData.CurrentMoisture,
                FieldCapacity = soilData.FieldCapacity,
                Temperature = soilData.Temperature,
                AvailableNitrogen = soilData.AvailableNitrogen,
                AvailablePhosphorus = soilData.AvailablePhosphorus,
                AvailablePotassium = soilData.AvailablePotassium,
            };
        }
    }
}
