using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AgroPlaner.DAL.Models
{
    public class FertilizationEvent
    {
        [Key]
        public int FertilizationEventId { get; set; }

        // Foreign key
        public int SoilDataId { get; set; }

        public DateTime Date { get; set; }
        public double NitrogenAmount { get; set; } // kg/ha
        public double PhosphorusAmount { get; set; } // kg/ha
        public double PotassiumAmount { get; set; } // kg/ha

        // Navigation property
        [ForeignKey("SoilDataId")]
        public virtual SoilData SoilData { get; set; }
    }
}
