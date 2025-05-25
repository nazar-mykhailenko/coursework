using AgroPlaner.DAL.Models;
using AgroPlaner.DAL.Repositories.Interfaces;
using AgroPlaner.Services.Interfaces;
using AgroPlaner.Services.Models;

namespace AgroPlaner.Services.Services
{
    public class PredictionsHelperService : IPredictionsHelperService
    {
        private readonly AgriPredictionService _agriPredictionService;
        private readonly ICropRepository _cropRepository;
        private readonly IWeatherDataService _weatherDataService;
        private readonly ISoilDataService _soilDataService;


        public PredictionsHelperService(
            AgriPredictionService agriPredictionService,
            ICropRepository cropRepository,
            IWeatherDataService weatherDataService,
            ISoilDataService soilDataService
        )
        {
            _agriPredictionService = agriPredictionService;
            _cropRepository = cropRepository;
            _weatherDataService = weatherDataService;
            _soilDataService = soilDataService;
        }

        public async Task<DateTime?> PredictSeedingDateByCoordinatesAsync(Crop crop, double latitude, double longitude)
        {
            // Get weather forecast for the coordinates
            var weatherForecast = await _weatherDataService.GetWeatherForecastByCoordinatesAsync(latitude, longitude);

            // Create a new SoilData instance with the data from the crop
            var soilData = new SoilData
            {
                Temperature = crop.Soil?.Temperature ?? 0,
                FieldCapacity = crop.Soil?.FieldCapacity ?? 0,
                CurrentMoisture = crop.Soil?.CurrentMoisture ?? 0,
                AvailableNitrogen = crop.Soil?.AvailableNitrogen ?? 0,
                AvailablePhosphorus = crop.Soil?.AvailablePhosphorus ?? 0,
                AvailablePotassium = crop.Soil?.AvailablePotassium ?? 0
            };

            return _agriPredictionService.PredictSeedingDate(
                crop,
                weatherForecast.Select(w => new WeatherData
                {
                    Date = w.Date,
                    MaxTemp = w.MaxTemp,
                    MinTemp = w.MinTemp,
                    Precipitation = w.Precipitation,
                    WindSpeed = w.WindSpeed,
                    SolarRadiation = w.SolarRadiation ?? 0,
                    RelativeHumidity = w.RelativeHumidity
                }).ToList(),
                soilData
            );
        }

        public async Task<(double Amount, DateTime? NextDate)> PredictIrrigationAsync(int cropId)
        {
            var crop = await _cropRepository.GetByIdAsync(cropId);
            if (crop == null)
            {
                return (0, null);
            }

            var soilData = await _soilDataService.GetByCropIdAsync(cropId);
            if (soilData == null)
            {
                return (0, null);
            }

            var weatherHistory = await _weatherDataService.GetHistoryByLocationIdAsync(
                crop.LocationId,
                DateTime.Today.AddDays(-30),
                DateTime.Today
            );

            var weatherForecast = await _weatherDataService.GetWeatherForecastAsync(crop.LocationId);
            var todayWeather = weatherForecast.FirstOrDefault();
            if (todayWeather == null)
            {
                return (0, null);
            }
            var currentStage = _agriPredictionService.GetCurrentGrowthStage(crop);
            var growthStageIndex = _agriPredictionService.GetGrowthStageIndex(currentStage, crop);

            var (amount, nextDate) = _agriPredictionService.CalculateIrrigation(
                crop,
                weatherHistory.ToList(),
                new WeatherData
                {
                    Date = todayWeather.Date,
                    MaxTemp = todayWeather.MaxTemp,
                    MinTemp = todayWeather.MinTemp,
                    Precipitation = todayWeather.Precipitation,
                    WindSpeed = todayWeather.WindSpeed,
                    SolarRadiation = todayWeather.SolarRadiation ?? 0,
                    RelativeHumidity = todayWeather.RelativeHumidity
                },
                soilData,
                growthStageIndex
            );

            // Validate that the irrigation date is realistic (within 1 year)
            if (nextDate.HasValue)
            {
                var daysFromToday = (nextDate.Value - DateTime.Today).TotalDays;
                if (daysFromToday < 0 || daysFromToday > 365)
                {
                    // Return no irrigation needed if date is unrealistic
                    return (0, null);
                }
            }

            return (amount, nextDate);
        }

        public async Task<(double NAmount, double PAmount, double KAmount, DateTime? NextDate)> PredictFertilizationAsync(int cropId)
        {
            var crop = await _cropRepository.GetByIdAsync(cropId);
            if (crop == null)
            {
                return (0, 0, 0, null);
            }

            var soilData = await _soilDataService.GetByCropIdAsync(cropId);
            if (soilData == null)
            {
                return (0, 0, 0, null);
            }

            if (crop.PlantingDate == null)
            {
                return (0, 0, 0, null);
            }

            var weatherHistory = await _weatherDataService.GetHistoryByLocationIdAsync(
                crop.LocationId,
                DateTime.Today.AddDays(-30),
                DateTime.Today
            ); var weatherForecast = await _weatherDataService.GetWeatherForecastAsync(crop.LocationId);

            var (nAmount, pAmount, kAmount, nextDate) = _agriPredictionService.CalculateFertilization(
                crop,
                soilData,
                crop.PlantingDate.Value,
                weatherHistory.ToList(),
                weatherForecast.Select(w => new WeatherData
                {
                    Date = w.Date,
                    MaxTemp = w.MaxTemp,
                    MinTemp = w.MinTemp,
                    Precipitation = w.Precipitation,
                    WindSpeed = w.WindSpeed,
                    SolarRadiation = w.SolarRadiation ?? 0,
                    RelativeHumidity = w.RelativeHumidity
                }).ToList()
            );

            // Validate that the fertilization date is realistic (within 1 year)
            if (nextDate.HasValue)
            {
                var daysFromToday = (nextDate.Value - DateTime.Today).TotalDays;
                if (daysFromToday < 0 || daysFromToday > 365)
                {
                    // Return no fertilization needed if date is unrealistic
                    return (0, 0, 0, null);
                }
            }

            return (nAmount, pAmount, kAmount, nextDate);
        }

        public async Task<DateTime?> PredictHarvestDateAsync(int cropId)
        {
            var crop = await _cropRepository.GetByIdAsync(cropId);
            if (crop == null)
            {
                return null;
            }

            var weatherForecast = await _weatherDataService.GetWeatherForecastAsync(crop.LocationId);

            return _agriPredictionService.PredictHarvestDate(
                crop,
                weatherForecast.Select(w => new WeatherData
                {
                    Date = w.Date,
                    MaxTemp = w.MaxTemp,
                    MinTemp = w.MinTemp,
                    Precipitation = w.Precipitation,
                    WindSpeed = w.WindSpeed,
                    SolarRadiation = w.SolarRadiation ?? 0,
                    RelativeHumidity = w.RelativeHumidity
                }).ToList()
            );
        }

        public async Task<PredictionResult> GetFullPredictionAsync(int cropId)
        {
            var crop = await _cropRepository.GetByIdAsync(cropId);
            if (crop == null)
            {
                return new PredictionResult();
            }

            var soilData = await _soilDataService.GetByCropIdAsync(cropId);
            if (soilData == null)
            {
                return new PredictionResult();
            }

            if (crop.PlantingDate == null)
            {
                return new PredictionResult();
            }

            var weatherHistory = await _weatherDataService.GetHistoryByLocationIdAsync(
                crop.LocationId,
                DateTime.Today.AddDays(-30),
                DateTime.Today
            ); var weatherForecast = await _weatherDataService.GetWeatherForecastAsync(crop.LocationId);

            var result = _agriPredictionService.Predict(
                crop,
                weatherHistory.ToList(),
                weatherForecast.Select(w => new WeatherData
                {
                    Date = w.Date,
                    MaxTemp = w.MaxTemp,
                    MinTemp = w.MinTemp,
                    Precipitation = w.Precipitation,
                    WindSpeed = w.WindSpeed,
                    SolarRadiation = w.SolarRadiation ?? 0,
                    RelativeHumidity = w.RelativeHumidity
                }).ToList(),
                crop.PlantingDate.Value
            );            // Validate that the irrigation date is realistic (within 1 year)
            if (result.NextIrrigationDate.HasValue)
            {
                var daysFromToday = (result.NextIrrigationDate.Value - DateTime.Today).TotalDays;
                if (daysFromToday < 0 || daysFromToday > 365)
                {
                    // Clear unrealistic irrigation data
                    result.NextIrrigationDate = null;
                    result.IrrigationAmount = 0;
                }
            }

            // Validate that the fertilization date is realistic (within 1 year)
            if (result.NextFertilizationDate.HasValue)
            {
                var daysFromToday = (result.NextFertilizationDate.Value - DateTime.Today).TotalDays;
                if (daysFromToday < 0 || daysFromToday > 365)
                {
                    // Clear unrealistic fertilization data
                    result.NextFertilizationDate = null;
                    result.NitrogenFertilizerAmount = 0;
                    result.PhosphorusFertilizerAmount = 0;
                    result.PotassiumFertilizerAmount = 0;
                }
            }

            return result;
        }
    }
}