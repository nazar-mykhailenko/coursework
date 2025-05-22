using AgroPlaner.DAL.Models;
using AgroPlaner.Services.Models;

namespace AgroPlaner.Services.Interfaces
{
    public interface IPredictionsHelperService
    {
        Task<DateTime?> PredictSeedingDateAsync(Crop crop, int locationId);
        Task<(double Amount, DateTime? NextDate)> PredictIrrigationAsync(int cropId);
        Task<(double NAmount, double PAmount, double KAmount, DateTime? NextDate)> PredictFertilizationAsync(int cropId);
        Task<DateTime?> PredictHarvestDateAsync(int cropId);
        Task<PredictionResult> GetFullPredictionAsync(int cropId);
    }
}