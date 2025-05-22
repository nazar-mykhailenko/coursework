using System.Linq.Expressions;
using AgroPlaner.DAL.Models;
using AgroPlaner.DAL.Repositories.Interfaces;
using AgroPlaner.Services.Interfaces;

namespace AgroPlaner.Services.Services
{
    public class CropService : ICropService
    {
        private readonly ICropRepository _cropRepository;
        private readonly ISoilDataRepository _soilDataRepository;

        public CropService(ICropRepository cropRepository, ISoilDataRepository soilDataRepository)
        {
            _cropRepository = cropRepository;
            _soilDataRepository = soilDataRepository;
        }

        public async Task<IEnumerable<Crop>> GetAllAsync(string? userId = null)
        {
            if (userId == null)
            {
                return await _cropRepository.GetAllAsync();
            }

            return await _cropRepository.GetFilteredAsync(c => c.UserId == userId);
        }

        public async Task<Crop?> GetByIdAsync(int id, string? userId = null)
        {
            var crop = await _cropRepository.GetByIdAsync(id);
            if (crop == null)
                return null;

            if (userId != null && crop.UserId != userId)
            {
                return null;
            }

            return crop;
        }
        public async Task<Crop> CreateAsync(Crop crop, string userId)
        {
            crop.UserId = userId;

            // Set planting date to current date when crop is created
            crop.PlantingDate = DateTime.Today;

            // Create a SoilData entry for this crop
            if (crop.Soil == null)
            {
                crop.Soil = new SoilData
                {
                    CurrentMoisture = 0,
                    FieldCapacity = 0,
                    Temperature = 0,
                    AvailableNitrogen = 0,
                    AvailablePhosphorus = 0,
                    AvailablePotassium = 0,
                };
            }

            return await _cropRepository.CreateAsync(crop);
        }

        public async Task<Crop> UpdateAsync(Crop crop, string userId)
        {
            var existingCrop = await _cropRepository.GetByIdAsync(crop.Id);
            if (existingCrop == null || existingCrop.UserId != userId)
            {
                throw new UnauthorizedAccessException(
                    "User does not have permission to update this crop"
                );
            }

            return await _cropRepository.UpdateAsync(crop);
        }

        public async Task<bool> DeleteAsync(int id, string userId)
        {
            var existingCrop = await _cropRepository.GetByIdAsync(id);
            if (existingCrop == null || existingCrop.UserId != userId)
            {
                return false;
            }

            await _cropRepository.DeleteAsync(id);
            return true;
        }

        public async Task<(IEnumerable<Crop> Items, int TotalCount)> GetPagedAsync(
            int pageNumber,
            int pageSize,
            string? userId = null
        )
        {
            Expression<Func<Crop, bool>>? filter = null;

            if (userId != null)
            {
                filter = c => c.UserId == userId;
            }

            return await _cropRepository.GetPagedAsync(pageNumber, pageSize, filter);
        }
    }
}
