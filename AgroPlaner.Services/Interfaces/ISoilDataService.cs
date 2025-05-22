using AgroPlaner.DAL.Models;

namespace AgroPlaner.Services.Interfaces
{
    public interface ISoilDataService
    {
        Task<SoilData?> GetByCropIdAsync(int cropId, string? userId = null);
        Task<SoilData> UpdateAsync(SoilData soilData, string userId);
        Task<SoilData> ApplyIrrigationEventAsync(
            int cropId,
            IrrigationEvent irrigationEvent,
            string userId
        );
        Task<SoilData> ApplyFertilizationEventAsync(
            int cropId,
            FertilizationEvent fertilizationEvent,
            string userId
        );
        Task<SoilData> CreateOrGetSoilDataAsync(int cropId, string userId);
    }
}
