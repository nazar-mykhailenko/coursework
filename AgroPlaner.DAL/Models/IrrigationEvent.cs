using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AgroPlaner.DAL.Models
{
    public class IrrigationEvent
    {
        [Key]
        public int IrrigationEventId { get; set; }

        // Foreign key
        public int SoilDataId { get; set; }

        public DateTime Date { get; set; }
        public double Amount { get; set; } // mm (per hectare)

        // Navigation property
        [ForeignKey("SoilDataId")]
        public virtual SoilData SoilData { get; set; }
    }
}
