using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AgroPlaner.DAL.Models
{
    public class Crop
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;

        // Foreign keys
        public int PlantId { get; set; }
        public int LocationId { get; set; }

        // Optional one-to-one relationship with SoilData
        public int? SoilDataId { get; set; }

        public double ExpectedYield { get; set; } // tons/ha
        public double CumulativeGDDToday { get; set; } // GDD accumulated up to today
        public double FieldArea { get; set; } // ha, area of the field
        public DateTime? PlantingDate { get; set; } // Date when the crop was planted

        // Navigation properties
        [ForeignKey("PlantId")]
        public virtual Plant Plant { get; set; }

        [ForeignKey("LocationId")]
        public virtual Location Location { get; set; }

        [ForeignKey("SoilDataId")]
        public virtual SoilData Soil { get; set; }

        // User relationship - one-to-many
        public string? UserId { get; set; }
        public virtual ApplicationUser? User { get; set; }

        // User relationship - many-to-many
    }
}
