namespace AgroPlaner.DAL.Models
{
    public class PredictionResult
    {
        public double IrrigationAmount { get; set; } // mm/ha
        public DateTime? NextIrrigationDate { get; set; }
        public double NitrogenFertilizerAmount { get; set; } // kg/ha
        public double PhosphorusFertilizerAmount { get; set; } // kg/ha
        public double PotassiumFertilizerAmount { get; set; } // kg/ha
        public DateTime? NextFertilizationDate { get; set; }
        public DateTime? PredictedHarvestDate { get; set; }
    }
}
