namespace AgroPlaner.Services.Models
{
    public class WeatherForecast
    {
        public int LocationId { get; set; }
        public DateTime Date { get; set; }
        public double MaxTemp { get; set; }
        public double MinTemp { get; set; }
        public double Precipitation { get; set; }
        public double WindSpeed { get; set; }
        public double? SolarRadiation { get; set; }
        public double RelativeHumidity { get; set; }
    }
}