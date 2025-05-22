using AgroPlaner.DAL.Models;
using AgroPlaner.DAL.Repositories.Interfaces;
using AgroPlaner.Services.Interfaces;

namespace AgroPlaner.Services.Services
{
    public class SoilDataService : ISoilDataService
    {
        private readonly ISoilDataRepository _soilDataRepository;
        private readonly ICropRepository _cropRepository;
        private readonly IEventRepository<IrrigationEvent> _irrigationEventRepository;
        private readonly IEventRepository<FertilizationEvent> _fertilizationEventRepository;

        public SoilDataService(
            ISoilDataRepository soilDataRepository,
            ICropRepository cropRepository,
            IEventRepository<IrrigationEvent> irrigationEventRepository,
            IEventRepository<FertilizationEvent> fertilizationEventRepository
        )
        {
            _soilDataRepository = soilDataRepository;
            _cropRepository = cropRepository;
            _irrigationEventRepository = irrigationEventRepository;
            _fertilizationEventRepository = fertilizationEventRepository;
        }
        public async Task<SoilData?> GetByCropIdAsync(int cropId, string? userId = null)
        {
            var crop = await _cropRepository.GetByIdAsync(cropId);
            if (crop == null || (userId != null && crop.UserId != userId))
            {
                return null;
            }

            // Return existing soil data if it exists
            if (crop.Soil != null)
            {
                return crop.Soil;
            }

            // If no soil data and userId is provided, create new soil data
            if (userId != null)
            {
                try
                {
                    return await CreateOrGetSoilDataAsync(cropId, userId);
                }
                catch
                {
                    // If creation fails, return null
                    return null;
                }
            }

            return null;
        }
        public async Task<SoilData> UpdateAsync(SoilData soilData, string userId)
        {
            var crop = await _cropRepository.GetByIdAsync(soilData.CropId);
            if (crop == null || crop.UserId != userId)
            {
                throw new UnauthorizedAccessException(
                    "User does not have permission to update this soil data"
                );
            }

            // If the crop doesn't have soil data yet, create it
            if (crop.SoilDataId == null)
            {
                soilData.SoilDataId = 0; // Let the repository handle the ID assignment for new entities
                var createdSoilData = await _soilDataRepository.CreateAsync(soilData);

                // Update the crop with the new soil data reference
                crop.SoilDataId = createdSoilData.SoilDataId;
                crop.Soil = createdSoilData;
                await _cropRepository.UpdateAsync(crop);

                return createdSoilData;
            }

            return await _soilDataRepository.UpdateAsync(soilData);
        }
        public async Task<SoilData> ApplyIrrigationEventAsync(
            int cropId,
            IrrigationEvent irrigationEvent,
            string userId
        )
        {
            var crop = await _cropRepository.GetByIdAsync(cropId);
            if (crop == null || crop.UserId != userId)
            {
                throw new UnauthorizedAccessException(
                    "User does not have permission to apply irrigation to this crop"
                );
            }

            // Ensure soil data exists for this crop
            var soilData = crop.Soil;
            if (soilData == null)
            {
                soilData = await CreateOrGetSoilDataAsync(cropId, userId);
            }

            irrigationEvent.SoilDataId = soilData.SoilDataId;
            irrigationEvent.Date = DateTime.Today;

            await _irrigationEventRepository.CreateAsync(irrigationEvent);
            soilData.ApplyIrrigation(irrigationEvent);

            return await _soilDataRepository.UpdateAsync(soilData);
        }
        public async Task<SoilData> ApplyFertilizationEventAsync(
            int cropId,
            FertilizationEvent fertilizationEvent,
            string userId
        )
        {
            var crop = await _cropRepository.GetByIdAsync(cropId);
            if (crop == null || crop.UserId != userId)
            {
                throw new UnauthorizedAccessException(
                    "User does not have permission to apply fertilization to this crop"
                );
            }

            // Ensure soil data exists for this crop
            var soilData = crop.Soil;
            if (soilData == null)
            {
                soilData = await CreateOrGetSoilDataAsync(cropId, userId);
            }

            fertilizationEvent.SoilDataId = soilData.SoilDataId;
            fertilizationEvent.Date = DateTime.Today;

            await _fertilizationEventRepository.CreateAsync(fertilizationEvent);
            soilData.ApplyFertilization(fertilizationEvent);

            return await _soilDataRepository.UpdateAsync(soilData);
        }

        public async Task<SoilData> CreateOrGetSoilDataAsync(int cropId, string userId)
        {
            var crop = await _cropRepository.GetByIdAsync(cropId);
            if (crop == null || crop.UserId != userId)
            {
                throw new UnauthorizedAccessException(
                    "User does not have permission to access this crop"
                );
            }

            // Return existing soil data if it exists
            if (crop.Soil != null)
            {
                return crop.Soil;
            }

            // Create new soil data if it doesn't exist
            var soilData = new SoilData
            {
                CropId = crop.Id,
                CurrentMoisture = 0,
                FieldCapacity = 0,
                Temperature = 0,
                AvailableNitrogen = 0,
                AvailablePhosphorus = 0,
                AvailablePotassium = 0,
            };

            // Create and save the new soil data
            var createdSoilData = await _soilDataRepository.CreateAsync(soilData);

            // Update the crop with the new soil data reference
            crop.SoilDataId = createdSoilData.SoilDataId;
            crop.Soil = createdSoilData;
            await _cropRepository.UpdateAsync(crop);

            return createdSoilData;
        }
    }
}
