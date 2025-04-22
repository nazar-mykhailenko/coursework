using System.Text.Json;
using AgroPlaner.DAL.Models;
using AgroPlaner.DAL.Repositories.Interfaces;
using AgroPlaner.Services.Interfaces;
using AgroPlaner.Services.Models;
using Microsoft.Extensions.Options;

namespace AgroPlaner.Services.Services
{
    public class WeatherDataService : IWeatherDataService
    {
        private readonly IWeatherDataRepository _weatherDataRepository;
        private readonly ILocationRepository _locationRepository;
        private readonly HttpClient _httpClient;
        private readonly WeatherApiSettings _apiSettings;

        public WeatherDataService(
            IWeatherDataRepository weatherDataRepository,
            ILocationRepository locationRepository,
            HttpClient httpClient,
            IOptions<WeatherApiSettings> apiSettings
        )
        {
            _weatherDataRepository = weatherDataRepository;
            _locationRepository = locationRepository;
            _httpClient = httpClient;
            _apiSettings = apiSettings.Value;
        }

        public async Task<IEnumerable<WeatherData>> GetHistoryByLocationIdAsync(
            int locationId,
            DateTime startDate,
            DateTime endDate
        )
        {
            // Assuming we have a method to filter weather data by location and date range
            // This would be implemented in a repository extension
            var allWeatherData = await _weatherDataRepository.GetAllAsync();
            return allWeatherData.Where(w =>
                w.LocationId == locationId && w.Date >= startDate && w.Date <= endDate
            );
        }

        public async Task<WeatherData?> GetTodayWeatherForLocationAsync(int locationId)
        {
            var allWeatherData = await _weatherDataRepository.GetAllAsync();
            var today = DateTime.Today;

            // Check if we already have today's weather
            var existingWeather = allWeatherData.FirstOrDefault(w =>
                w.LocationId == locationId && w.Date.Date == today
            );

            if (existingWeather != null)
            {
                return existingWeather;
            }

            // If not, fetch it from the API and save it
            var location = await _locationRepository.GetByIdAsync(locationId);
            if (location == null)
            {
                return null;
            }

            var weatherData = await FetchWeatherFromApiAsync(location);
            if (weatherData != null)
            {
                await _weatherDataRepository.CreateAsync(weatherData);
                return weatherData;
            }

            return null;
        }

        public async Task UpdateWeatherForAllLocationsAsync()
        {
            var locations = await _locationRepository.GetAllAsync();
            var today = DateTime.Today;

            foreach (var location in locations)
            {
                var weatherData = await FetchWeatherFromApiAsync(location);
                if (weatherData != null)
                {
                    // Check if we already have weather for this location and date
                    var allWeatherData = await _weatherDataRepository.GetAllAsync();
                    var existingWeather = allWeatherData.FirstOrDefault(w =>
                        w.LocationId == location.LocationId && w.Date.Date == today
                    );

                    if (existingWeather != null)
                    {
                        // Update existing weather data
                        existingWeather.MaxTemp = weatherData.MaxTemp;
                        existingWeather.MinTemp = weatherData.MinTemp;
                        existingWeather.Precipitation = weatherData.Precipitation;
                        existingWeather.WindSpeed = weatherData.WindSpeed;
                        existingWeather.SolarRadiation = weatherData.SolarRadiation;
                        existingWeather.RelativeHumidity = weatherData.RelativeHumidity;

                        await _weatherDataRepository.UpdateAsync(existingWeather);
                    }
                    else
                    {
                        // Create new weather data
                        await _weatherDataRepository.CreateAsync(weatherData);
                    }
                }
            }
        }

        private async Task<WeatherData?> FetchWeatherFromApiAsync(Location location)
        {
            try
            {
                // Example using OpenWeatherMap API
                string url =
                    $"{_apiSettings.BaseUrl}?lat={location.Latitude}&lon={location.Longitude}&appid={_apiSettings.ApiKey}&units=metric";

                var response = await _httpClient.GetAsync(url);
                response.EnsureSuccessStatusCode();

                var responseContent = await response.Content.ReadAsStringAsync();
                var weatherResponse = JsonSerializer.Deserialize<OpenWeatherMapResponse>(
                    responseContent
                );

                if (weatherResponse != null)
                {
                    return new WeatherData
                    {
                        LocationId = location.LocationId,
                        Date = DateTime.Today,
                        MaxTemp = weatherResponse.Main.TempMax,
                        MinTemp = weatherResponse.Main.TempMin,
                        Precipitation = weatherResponse.Rain?.ThreeHours ?? 0,
                        WindSpeed = weatherResponse.Wind.Speed,
                        SolarRadiation = CalculateSolarRadiationEstimate(
                            location.Latitude,
                            location.Longitude,
                            DateTime.Today
                        ),
                        RelativeHumidity = weatherResponse.Main.Humidity,
                    };
                }
            }
            catch (Exception ex)
            {
                // Log the exception
                Console.WriteLine($"Error fetching weather data: {ex.Message}");
            }

            return null;
        }

        // Simple estimate for solar radiation when not available from API
        private double CalculateSolarRadiationEstimate(
            double latitude,
            double longitude,
            DateTime date
        )
        {
            // This is a simplified placeholder calculation
            // In a real implementation, you would use a proper solar radiation model
            int dayOfYear = date.DayOfYear;
            double latitudeRad = latitude * Math.PI / 180;

            // Declination angle
            double declination = 23.45 * Math.Sin(Math.PI * (284 + dayOfYear) / 182.5);
            double declinationRad = declination * Math.PI / 180;

            // Day length in hours
            double dayLength =
                2 * Math.Acos(-Math.Tan(latitudeRad) * Math.Tan(declinationRad)) * 12 / Math.PI;

            // Simple estimate based on day length and season
            double baseRadiation = 15.0; // MJ/m²/day
            double seasonalFactor = 1.0 + 0.5 * Math.Sin(Math.PI * (dayOfYear - 81) / 182.5);

            return baseRadiation * seasonalFactor * (dayLength / 12.0);
        }

        // Classes to deserialize OpenWeatherMap API response
        private class OpenWeatherMapResponse
        {
            public MainData Main { get; set; } = new MainData();
            public WindData Wind { get; set; } = new WindData();
            public RainData? Rain { get; set; }
        }

        private class MainData
        {
            public double Temp { get; set; }
            public double TempMin { get; set; }
            public double TempMax { get; set; }
            public double Humidity { get; set; }
        }

        private class WindData
        {
            public double Speed { get; set; }
        }

        private class RainData
        {
            [System.Text.Json.Serialization.JsonPropertyName("3h")]
            public double ThreeHours { get; set; }
        }
    }
}
