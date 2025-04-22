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

            return crop.Soil;
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

            var soilData = crop.Soil;
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

            var soilData = crop.Soil;
            fertilizationEvent.SoilDataId = soilData.SoilDataId;
            fertilizationEvent.Date = DateTime.Today;

            await _fertilizationEventRepository.CreateAsync(fertilizationEvent);
            soilData.ApplyFertilization(fertilizationEvent);

            return await _soilDataRepository.UpdateAsync(soilData);
        }
    }
}
