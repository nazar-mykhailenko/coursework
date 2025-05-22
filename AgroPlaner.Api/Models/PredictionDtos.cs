namespace AgroPlaner.Api.Models
{
    public class SeedingPredictionRequestDto
    {
        public int PlantId { get; set; }
        public int LocationId { get; set; }
        public double ExpectedYield { get; set; }
        public double FieldArea { get; set; }
        public SoilDataDto Soil { get; set; } = new();
    }

    public class SeedingPredictionResponseDto
    {
        public DateTime? RecommendedSeedingDate { get; set; }
    }

    public class IrrigationPredictionResponseDto
    {
        public double Amount { get; set; }
        public DateTime? NextDate { get; set; }
    }

    public class FertilizationPredictionResponseDto
    {
        public double NitrogenAmount { get; set; }
        public double PhosphorusAmount { get; set; }
        public double PotassiumAmount { get; set; }
        public DateTime? NextDate { get; set; }
    }

    public class HarvestPredictionResponseDto
    {
        public DateTime? PredictedHarvestDate { get; set; }
    }

    public class FullPredictionResponseDto
    {
        public DateTime? RecommendedSeedingDate { get; set; }
        public double IrrigationAmount { get; set; }
        public DateTime? NextIrrigationDate { get; set; }
        public double NitrogenFertilizerAmount { get; set; }
        public double PhosphorusFertilizerAmount { get; set; }
        public double PotassiumFertilizerAmount { get; set; }
        public DateTime? NextFertilizationDate { get; set; }
        public DateTime? PredictedHarvestDate { get; set; }
    }
}