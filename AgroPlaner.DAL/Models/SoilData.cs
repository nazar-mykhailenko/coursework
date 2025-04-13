using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AgroPlaner.DAL.Models
{
    public class SoilData
    {
        [Key]
        public int SoilDataId { get; set; }

        // Foreign key to crop
        public int CropId { get; set; }

        public double CurrentMoisture { get; set; } // mm (per hectare)
        public double FieldCapacity { get; set; } // mm (per hectare)
        public double Temperature { get; set; } // °C
        public double AvailableNitrogen { get; set; } // kg/ha
        public double AvailablePhosphorus { get; set; } // kg/ha (P2O5)
        public double AvailablePotassium { get; set; } // kg/ha (K2O)

        // Navigation properties
        [ForeignKey("CropId")]
        public virtual Crop Crop { get; set; }

        public virtual ICollection<FertilizationEvent> FertilizationHistory { get; set; } =
            new List<FertilizationEvent>();
        public virtual ICollection<IrrigationEvent> IrrigationHistory { get; set; } =
            new List<IrrigationEvent>();

        public void ApplyFertilization(FertilizationEvent fertEvent)
        {
            AvailableNitrogen += fertEvent.NitrogenAmount * 0.9; // 90% efficiency
            AvailablePhosphorus += fertEvent.PhosphorusAmount * 0.8; // 80% efficiency
            AvailablePotassium += fertEvent.PotassiumAmount * 0.85; // 85% efficiency
            FertilizationHistory.Add(fertEvent);
        }

        public void ApplyIrrigation(IrrigationEvent irrEvent)
        {
            CurrentMoisture = Math.Min(CurrentMoisture + irrEvent.Amount * 0.9, FieldCapacity); // 90% efficiency
            IrrigationHistory.Add(irrEvent);
        }
    }
}
