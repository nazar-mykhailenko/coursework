using AgroPlaner.DAL.Models;
using AgroPlaner.DAL.Repositories.Interfaces;
using AgroPlaner.Services.Interfaces;

namespace AgroPlaner.Services.Services
{
    public class PlantService : IPlantService
    {
        private readonly IPlantRepository _plantRepository;

        public PlantService(IPlantRepository plantRepository)
        {
            _plantRepository = plantRepository;
        }

        public async Task<IEnumerable<Plant>> GetAllAsync()
        {
            return await _plantRepository.GetAllAsync();
        }

        public async Task<Plant?> GetByIdAsync(int plantId)
        {
            return await _plantRepository.GetByIdAsync(plantId);
        }
    }
}
