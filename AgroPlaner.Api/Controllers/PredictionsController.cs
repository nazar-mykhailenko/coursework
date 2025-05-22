using AgroPlaner.Api.Models;
using AgroPlaner.DAL.Models;
using AgroPlaner.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AgroPlaner.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class PredictionsController : ControllerBase
    {
        private readonly IPredictionsHelperService _predictionsService;
        private readonly IPlantService _plantService;

        public PredictionsController(
            IPredictionsHelperService predictionsService,
            IPlantService plantService
        )
        {
            _predictionsService = predictionsService;
            _plantService = plantService;
        }

        [HttpPost("seeding")]
        public async Task<ActionResult<SeedingPredictionResponseDto>> PredictSeedingDate(
            [FromBody] SeedingPredictionRequestDto request
        )
        {
            var plant = await _plantService.GetByIdAsync(request.PlantId);
            if (plant == null)
            {
                return NotFound($"Plant with ID {request.PlantId} not found");
            }

            var crop = new Crop
            {
                Plant = plant,
                LocationId = request.LocationId,
                ExpectedYield = request.ExpectedYield,
                FieldArea = request.FieldArea,
                Soil = new SoilData
                {
                    Temperature = request.Soil.Temperature,
                    FieldCapacity = request.Soil.FieldCapacity,
                    CurrentMoisture = request.Soil.CurrentMoisture,
                    AvailableNitrogen = request.Soil.AvailableNitrogen,
                    AvailablePhosphorus = request.Soil.AvailablePhosphorus,
                    AvailablePotassium = request.Soil.AvailablePotassium
                }
            };

            var seedingDate = await _predictionsService.PredictSeedingDateAsync(
                crop,
                request.LocationId
            );

            return Ok(new SeedingPredictionResponseDto { RecommendedSeedingDate = seedingDate });
        }

        [HttpGet("irrigation/{cropId}")]
        public async Task<ActionResult<IrrigationPredictionResponseDto>> PredictIrrigation(
            int cropId
        )
        {
            var (amount, nextDate) = await _predictionsService.PredictIrrigationAsync(cropId);
            return Ok(new IrrigationPredictionResponseDto { Amount = amount, NextDate = nextDate });
        }

        [HttpGet("fertilization/{cropId}")]
        public async Task<ActionResult<FertilizationPredictionResponseDto>> PredictFertilization(
            int cropId
        )
        {
            var (nAmount, pAmount, kAmount, nextDate) = await _predictionsService
                .PredictFertilizationAsync(cropId);
            return Ok(
                new FertilizationPredictionResponseDto
                {
                    NitrogenAmount = nAmount,
                    PhosphorusAmount = pAmount,
                    PotassiumAmount = kAmount,
                    NextDate = nextDate
                }
            );
        }

        [HttpGet("harvest/{cropId}")]
        public async Task<ActionResult<HarvestPredictionResponseDto>> PredictHarvestDate(int cropId)
        {
            var harvestDate = await _predictionsService.PredictHarvestDateAsync(cropId);
            return Ok(new HarvestPredictionResponseDto { PredictedHarvestDate = harvestDate });
        }

        [HttpGet("full/{cropId}")]
        public async Task<ActionResult<FullPredictionResponseDto>> GetFullPrediction(int cropId)
        {
            var prediction = await _predictionsService.GetFullPredictionAsync(cropId);
            return Ok(
                new FullPredictionResponseDto
                {
                    RecommendedSeedingDate = prediction.RecommendedSeedingDate,
                    IrrigationAmount = prediction.IrrigationAmount,
                    NextIrrigationDate = prediction.NextIrrigationDate,
                    NitrogenFertilizerAmount = prediction.NitrogenFertilizerAmount,
                    PhosphorusFertilizerAmount = prediction.PhosphorusFertilizerAmount,
                    PotassiumFertilizerAmount = prediction.PotassiumFertilizerAmount,
                    NextFertilizationDate = prediction.NextFertilizationDate,
                    PredictedHarvestDate = prediction.PredictedHarvestDate
                }
            );
        }
    }
}