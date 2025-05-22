namespace AgroPlaner.Services.Models
{
    public class WeatherApiSettings
    {
        public string ApiKey { get; set; } = string.Empty;
        public string BaseUrl { get; set; } = string.Empty;
        public int CacheDurationMinutes { get; set; } = 60;
    }

    public class NasaPowerApiSettings
    {
        public string BaseUrl { get; set; } = "https://power.larc.nasa.gov/api/temporal/daily/point";
    }
}
