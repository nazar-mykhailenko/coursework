using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;
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
        private readonly NasaPowerApiSettings _nasaPowerSettings;

        public WeatherDataService(
            IWeatherDataRepository weatherDataRepository,
            ILocationRepository locationRepository,
            HttpClient httpClient,
            IOptions<WeatherApiSettings> apiSettings,
            IOptions<NasaPowerApiSettings> nasaPowerSettings
        )
        {
            _weatherDataRepository = weatherDataRepository;
            _locationRepository = locationRepository;
            _httpClient = httpClient;
            _apiSettings = apiSettings.Value;
            _nasaPowerSettings = nasaPowerSettings.Value;
        }

        public async Task<IEnumerable<WeatherData>> GetHistoryByLocationIdAsync(
            int locationId,
            DateTime startDate,
            DateTime endDate
        )
        {
            var allWeatherData = await _weatherDataRepository.GetAllAsync();
            return allWeatherData.Where(w =>
                w.LocationId == locationId && w.Date >= startDate && w.Date <= endDate
            );
        }

        public async Task<WeatherData?> GetTodayWeatherForLocationAsync(int locationId)
        {
            var allWeatherData = await _weatherDataRepository.GetAllAsync();
            var today = DateTime.Today;

            var existingWeather = allWeatherData.FirstOrDefault(w =>
                w.LocationId == locationId && w.Date.Date == today
            );

            if (existingWeather != null)
            {
                return existingWeather;
            }

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
                    var allWeatherData = await _weatherDataRepository.GetAllAsync();
                    var existingWeather = allWeatherData.FirstOrDefault(w =>
                        w.LocationId == location.LocationId && w.Date.Date == today
                    );

                    if (existingWeather != null)
                    {
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
                        await _weatherDataRepository.CreateAsync(weatherData);
                    }
                }
            }
        }

        public async Task<WeatherData?> FetchWeatherFromApiAsync(Location location)
        {
            try
            {
                if (!IsValidCoordinate(location.Latitude, location.Longitude))
                {
                    Console.WriteLine($"Invalid coordinates: Latitude={location.Latitude}, Longitude={location.Longitude}");
                    return null;
                }

                string latitude = location.Latitude.ToString(CultureInfo.InvariantCulture);
                string longitude = location.Longitude.ToString(CultureInfo.InvariantCulture);

                // Construct API URL for OpenWeatherMap forecast instead of current weather
                string url = $"{_apiSettings.BaseUrl}/forecast?lat={latitude}&lon={longitude}&appid={_apiSettings.ApiKey}&units=metric";
                Console.WriteLine($"Making request to OpenWeatherMap forecast API: {url}");

                var response = await _httpClient.GetAsync(url);

                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($"OpenWeatherMap API request failed with status code {response.StatusCode}: {errorContent}");
                    return null;
                }

                var responseContent = await response.Content.ReadAsStringAsync();
                var forecastResponse = JsonSerializer.Deserialize<OpenWeatherMapForecastResponse>(responseContent);

                if (forecastResponse?.List == null || !forecastResponse.List.Any())
                {
                    Console.WriteLine("Invalid or empty response from OpenWeatherMap API");
                    return null;
                }

                // Filter forecast items for today
                var todaysForecast = forecastResponse.List
                    .Where(f => f.DtTxt.Date == DateTime.Today)
                    .ToList();

                if (!todaysForecast.Any())
                {
                    Console.WriteLine("No forecast data available for today");
                    return null;
                }

                // Calculate aggregated values for the day
                var weatherData = new WeatherData
                {
                    LocationId = location.LocationId,
                    Date = DateTime.Today,
                    MaxTemp = todaysForecast.Max(f => f.Main.TempMax),
                    MinTemp = todaysForecast.Min(f => f.Main.TempMin),
                    WindSpeed = todaysForecast.Average(f => f.Wind.Speed),
                    RelativeHumidity = todaysForecast.Average(f => f.Main.Humidity),
                    Precipitation = todaysForecast.Sum(f => f.Rain?.ThreeHours ?? 0)
                };

                // Calculate solar radiation using the existing method
                CalculateSolarRadiation(weatherData, location.Latitude);

                return weatherData;
            }
            catch (JsonException ex)
            {
                Console.WriteLine($"JSON deserialization error: {ex.Message}");
                return null;
            }
            catch (HttpRequestException ex)
            {
                Console.WriteLine($"HTTP request error: {ex.Message}");
                return null;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Unexpected error fetching weather data: {ex.Message}");
                return null;
            }
        }

        private bool IsValidCoordinate(double latitude, double longitude)
        {
            return latitude >= -90 && latitude <= 90 && longitude >= -180 && longitude <= 180;
        }

        public void CalculateSolarRadiation(WeatherData weatherData, double latitude)
        {
            int n = weatherData.Date.DayOfYear;
            double T_max = weatherData.MaxTemp;
            double T_min = weatherData.MinTemp;

            double S0 = 0.0820;
            double kr = 0.17;

            double phi = latitude * Math.PI / 180;
            double delta = 0.409 * Math.Sin(2 * Math.PI * (284 + n) / 365);
            double omega_s = Math.Acos(-Math.Tan(phi) * Math.Tan(delta));
            double dr = 1 + 0.033 * Math.Cos(2 * Math.PI * n / 365);

            double G0 = (24 * 60 / Math.PI) * S0 * dr * (Math.Cos(phi) * Math.Cos(delta) * Math.Sin(omega_s) + (Math.PI * omega_s / 180) * Math.Sin(phi) * Math.Sin(delta));

            double G = kr * Math.Sqrt(T_max - T_min) * G0;

            weatherData.SolarRadiation = G;
        }

        public async Task<IEnumerable<WeatherForecast>> GetWeatherForecastAsync(int locationId, int days = 30)
        {
            try
            {
                var location = await _locationRepository.GetByIdAsync(locationId);
                if (location == null)
                {
                    return Enumerable.Empty<WeatherForecast>();
                }

                // Updated URL to use the correct climate API endpoint
                string url = $"https://pro.openweathermap.org/data/2.5/forecast/climate?lat={location.Latitude.ToString(CultureInfo.InvariantCulture)}&lon={location.Longitude.ToString(CultureInfo.InvariantCulture)}&appid={_apiSettings.ApiKey}&units=metric&cnt={days}";

                var response = await _httpClient.GetAsync(url);
                response.EnsureSuccessStatusCode();

                var responseContent = await response.Content.ReadAsStringAsync();
                var climateResponse = JsonSerializer.Deserialize<OpenWeatherMapClimateResponse>(responseContent);

                if (climateResponse?.List != null)
                {
                    return climateResponse.List.Select(item => new WeatherForecast
                    {
                        LocationId = locationId,
                        Date = DateTimeOffset.FromUnixTimeSeconds(item.Dt).Date,
                        MaxTemp = item.Temp.Max,
                        MinTemp = item.Temp.Min,
                        Precipitation = item.Rain.GetValueOrDefault(), // If Rain is null, use 0
                        WindSpeed = item.Speed,
                        SolarRadiation = null, // Climate API doesn't provide solar radiation
                        RelativeHumidity = item.Humidity
                    });
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching weather forecast: {ex.Message}");
            }

            return Enumerable.Empty<WeatherForecast>();
        }

        private class OpenWeatherMapClimateResponse
        {
            [JsonPropertyName("list")]
            public List<ClimateData> List { get; set; } = new();
        }

        private class ClimateData
        {
            [JsonPropertyName("dt")]
            public long Dt { get; set; }

            [JsonPropertyName("temp")]
            public TemperatureData Temp { get; set; } = new();

            [JsonPropertyName("humidity")]
            public double Humidity { get; set; }

            [JsonPropertyName("speed")]
            public double Speed { get; set; }

            [JsonPropertyName("rain")]
            public double? Rain { get; set; }
        }

        private class TemperatureData
        {
            [JsonPropertyName("day")]
            public double Day { get; set; }

            [JsonPropertyName("min")]
            public double Min { get; set; }

            [JsonPropertyName("max")]
            public double Max { get; set; }

            [JsonPropertyName("night")]
            public double Night { get; set; }

            [JsonPropertyName("eve")]
            public double Eve { get; set; }

            [JsonPropertyName("morn")]
            public double Morn { get; set; }
        }

        private class OpenWeatherMapForecastResponse
        {
            [JsonPropertyName("list")]
            public List<ForecastData> List { get; set; } = new();
        }

        private class ForecastData
        {
            [JsonPropertyName("dt")]
            public long Dt { get; set; }

            [JsonIgnore]
            public DateTime DtTxt => DateTimeOffset.FromUnixTimeSeconds(Dt).DateTime;

            [JsonPropertyName("main")]
            public MainData Main { get; set; } = new();

            [JsonPropertyName("wind")]
            public WindData Wind { get; set; } = new();

            [JsonPropertyName("rain")]
            public RainData? Rain { get; set; }
        }

        private class MainData
        {
            [JsonPropertyName("temp")]
            public double Temp { get; set; }
            [JsonPropertyName("temp_min")]
            public double TempMin { get; set; }
            [JsonPropertyName("temp_max")]
            public double TempMax { get; set; }
            [JsonPropertyName("humidity")]
            public double Humidity { get; set; }
        }

        private class WindData
        {
            [JsonPropertyName("speed")]
            public double Speed { get; set; }
        }

        private class RainData
        {
            [JsonPropertyName("3h")]
            public double ThreeHours { get; set; }
        }
    }
}
