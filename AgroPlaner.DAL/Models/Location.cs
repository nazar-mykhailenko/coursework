using System.ComponentModel.DataAnnotations;

namespace AgroPlaner.DAL.Models
{
    public class Location
    {
        [Key]
        public int LocationId { get; set; }
        public string Name { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }

        // User relationship
        public string? UserId { get; set; }
        public virtual ApplicationUser? User { get; set; }

        // Navigation properties
        public virtual ICollection<WeatherData> WeatherHistory { get; set; } =
            new List<WeatherData>();
        public virtual ICollection<Crop> Crops { get; set; } = new List<Crop>();
    }
}
