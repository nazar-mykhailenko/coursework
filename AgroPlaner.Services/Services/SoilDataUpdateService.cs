using AgroPlaner.DAL.Models;
using AgroPlaner.DAL.Repositories.Interfaces;
using AgroPlaner.Services.Interfaces;
using Microsoft.Extensions.Logging;

namespace AgroPlaner.Services.Services
{
    public interface ISoilDataUpdateService
    {
        Task UpdateAllSoilDataAsync();
        Task UpdateSoilDataForCropAsync(int cropId);
    }

    public class SoilDataUpdateService : ISoilDataUpdateService
    {
        private readonly ILogger<SoilDataUpdateService> _logger;
        private readonly ICropRepository _cropRepository;
        private readonly ISoilDataService _soilDataService;
        private readonly IWeatherDataService _weatherDataService;

        public SoilDataUpdateService(
            ILogger<SoilDataUpdateService> logger,
            ICropRepository cropRepository,
            ISoilDataService soilDataService,
            IWeatherDataService weatherDataService
        )
        {
            _logger = logger;
            _cropRepository = cropRepository;
            _soilDataService = soilDataService;
            _weatherDataService = weatherDataService;
        }

        public async Task UpdateAllSoilDataAsync()
        {
            _logger.LogInformation("Starting soil data update for all crops.");

            var crops = await _cropRepository.GetAllAsync();
            foreach (var crop in crops)
            {
                try
                {
                    await UpdateSoilDataForCropAsync(crop.Id);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"Error updating soil data for crop {crop.Id}");
                }
            }

            _logger.LogInformation("Completed soil data update for all crops.");
        }

        public async Task UpdateSoilDataForCropAsync(int cropId)
        {
            var crop = await _cropRepository.GetByIdAsync(cropId);
            if (crop == null)
            {
                _logger.LogWarning($"Crop with ID {cropId} not found");
                return;
            }

            var weatherData = await _weatherDataService.GetTodayWeatherForLocationAsync(crop.LocationId);
            if (weatherData == null)
            {
                _logger.LogWarning($"No weather data available for location {crop.LocationId}");
                return;
            }

            var soilData = crop.Soil;
            if (soilData == null)
            {
                _logger.LogWarning($"No soil data available for crop {cropId}");
                return;
            }

            // Update soil temperature based on air temperature with a lag
            UpdateSoilTemperature(soilData, weatherData);

            // Update soil moisture based on precipitation and evapotranspiration
            UpdateSoilMoisture(soilData, weatherData, crop);

            // Update nutrient availability based on temperature and moisture
            UpdateNutrientAvailability(soilData, weatherData);

            if (crop.UserId == null)
            {
                _logger.LogWarning($"Cannot update soil data for crop {cropId}: UserId is null");
                return;
            }

            await _soilDataService.UpdateAsync(soilData, crop.UserId);
            _logger.LogInformation($"Updated soil data for crop {cropId}");
        }

        private void UpdateSoilTemperature(SoilData soilData, WeatherData weatherData)
        {
            const double SOIL_TEMP_MEMORY_FACTOR = 0.7;
            const double AIR_TEMP_INFLUENCE_FACTOR = 0.3;

            double avgAirTemp = (weatherData.MaxTemp + weatherData.MinTemp) / 2;
            soilData.Temperature = SOIL_TEMP_MEMORY_FACTOR * soilData.Temperature + AIR_TEMP_INFLUENCE_FACTOR * avgAirTemp;
        }

        private void UpdateSoilMoisture(SoilData soilData, WeatherData weatherData, Crop crop)
        {
            const double EVAPORATION_FACTOR = 0.7;
            const double INFILTRATION_FACTOR = 0.8;

            // Calculate potential evapotranspiration
            double pet = CalculatePotentialEvapotranspiration(weatherData, crop);

            // Calculate actual evapotranspiration based on soil moisture
            double aet = Math.Min(pet, soilData.CurrentMoisture * EVAPORATION_FACTOR);

            // Calculate infiltration from precipitation
            double infiltration = weatherData.Precipitation * INFILTRATION_FACTOR;

            // Update soil moisture
            soilData.CurrentMoisture = Math.Max(0, soilData.CurrentMoisture - aet + infiltration);
            soilData.CurrentMoisture = Math.Min(soilData.CurrentMoisture, soilData.FieldCapacity);
        }

        private void UpdateNutrientAvailability(SoilData soilData, WeatherData weatherData)
        {
            const double TEMP_FACTOR = 0.02;
            const double MOISTURE_FACTOR = 0.01;
            const double MIN_TEMP_FOR_MICROBIAL_ACTIVITY = 5.0;

            // Calculate microbial activity factor based on temperature and moisture
            double tempFactor = Math.Max(0, (weatherData.MaxTemp - MIN_TEMP_FOR_MICROBIAL_ACTIVITY) * TEMP_FACTOR);
            double moistureFactor = (soilData.CurrentMoisture / soilData.FieldCapacity) * MOISTURE_FACTOR;
            double microbialActivity = tempFactor * moistureFactor;

            // Update nutrient availability based on microbial activity
            // More microbial activity means more nutrients become available
            soilData.AvailableNitrogen *= (1 + microbialActivity);
            soilData.AvailablePhosphorus *= (1 + microbialActivity * 0.8); // Phosphorus is less mobile
            soilData.AvailablePotassium *= (1 + microbialActivity * 0.9); // Potassium is moderately mobile
        }

        private double CalculatePotentialEvapotranspiration(WeatherData weatherData, Crop crop)
        {
            const double SOLAR_RADIATION_FACTOR = 0.0023;
            const double WIND_FACTOR = 0.34;
            const double HUMIDITY_FACTOR = 0.14;

            double avgTemp = (weatherData.MaxTemp + weatherData.MinTemp) / 2;
            double solarRadiation = weatherData.SolarRadiation;
            double windSpeed = weatherData.WindSpeed;
            double humidity = weatherData.RelativeHumidity;

            // Simplified Hargreaves-Samani equation
            double pet = SOLAR_RADIATION_FACTOR * solarRadiation * (avgTemp + 17.8) * Math.Sqrt(weatherData.MaxTemp - weatherData.MinTemp);

            // Adjust for wind and humidity
            pet *= (1 + WIND_FACTOR * windSpeed) * (1 - HUMIDITY_FACTOR * humidity / 100);

            return pet;
        }
    }
}