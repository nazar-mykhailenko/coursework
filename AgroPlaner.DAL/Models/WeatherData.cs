using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AgroPlaner.DAL.Models
{
    public class WeatherData
    {
        [Key]
        public int WeatherDataId { get; set; }

        // Foreign key
        public int LocationId { get; set; }

        public DateTime Date { get; set; }
        public double MaxTemp { get; set; } // °C
        public double MinTemp { get; set; } // °C
        public double Precipitation { get; set; } // mm
        public double WindSpeed { get; set; } // m/s
        public double SolarRadiation { get; set; } // MJ/m²/day
        public double RelativeHumidity { get; set; } // %

        // Navigation property
        [ForeignKey("LocationId")]
        public virtual Location Location { get; set; }
    }
}
