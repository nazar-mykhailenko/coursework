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
        public async Task<ActionResult<IEnumerable<Plant>>> GetPlants()
        {
            var plants = await _plantService.GetAllAsync();
            return Ok(plants);
        }

        // GET: api/Plants/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Plant>> GetPlant(int id)
        {
            var plant = await _plantService.GetByIdAsync(id);

            if (plant == null)
            {
                return NotFound();
            }

            return Ok(plant);
        }

        // Note: Since Plant is defined as a static table with data,
        // we don't implement POST, PUT or DELETE endpoints
    }
}
