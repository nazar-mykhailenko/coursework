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
    public class PlantsController : ControllerBase
    {
        private readonly IPlantService _plantService;

        public PlantsController(IPlantService plantService)
        {
            _plantService = plantService;
        }

        // GET: api/Plants
        [HttpGet]
        public async Task<ActionResult<IEnumerable<PlantDto>>> GetPlants()
        {
            var plants = await _plantService.GetAllAsync();
            var dtos = plants.Select(MapPlantToDto);
            return Ok(dtos);
        }

        // GET: api/Plants/5
        [HttpGet("{id}")]
        public async Task<ActionResult<PlantDto>> GetPlant(int id)
        {
            var plant = await _plantService.GetByIdAsync(id);

            if (plant == null)
            {
                return NotFound();
            }

            return Ok(MapPlantToDto(plant));
        }

        // Note: Since Plant is defined as a static table with data,
        // we don't implement POST, PUT or DELETE endpoints

        // Helper method to map Plant domain model to PlantDto
        private static PlantDto MapPlantToDto(Plant plant)
        {
            return new PlantDto
            {
                PlantId = plant.PlantId,
                Name = plant.Name,
                MinSoilTempForSeeding = plant.MinSoilTempForSeeding,
                BaseTempForGDD = plant.BaseTempForGDD,
                MaturityGDD = plant.MaturityGDD,
                RootDepth = plant.RootDepth,
                AllowableDepletionFraction = plant.AllowableDepletionFraction,
                NitrogenContent = plant.NitrogenContent,
                PhosphorusContent = plant.PhosphorusContent,
                PotassiumContent = plant.PotassiumContent,
            };
        }
    }
}
