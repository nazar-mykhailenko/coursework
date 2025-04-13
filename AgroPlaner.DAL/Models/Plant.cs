using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AgroPlaner.DAL.Models
{
    public class Plant
    {
        [Key]
        public int PlantId { get; set; }
        public string Name { get; set; }
        public double MinSoilTempForSeeding { get; set; } // °C
        public double BaseTempForGDD { get; set; } // °C
        public double MaturityGDD { get; set; } // Cumulative GDD for harvest
        public double[] KcValues { get; set; } // Crop coefficient for each growth stage
        public double RootDepth { get; set; } // m

        // Simple collection property that EF Core can map
        public virtual ICollection<PlantGrowthStage> PlantGrowthStages { get; set; } =
            new List<PlantGrowthStage>();

        public double AllowableDepletionFraction { get; set; } // Fraction of TAW that can be depleted (p)
        public double NitrogenContent { get; set; } // kg/ha per unit yield
        public double PhosphorusContent { get; set; } // kg/ha per unit yield (P2O5)
        public double PotassiumContent { get; set; } // kg/ha per unit yield (K2O)
        public double[] NitrogenUptakeDistribution { get; set; } // % of total N per stage
        public double[] PhosphorusUptakeDistribution { get; set; } // % of total P per stage
        public double[] PotassiumUptakeDistribution { get; set; } // % of total K per stage
        public string FertilizerType { get; set; } // e.g., "10-20-20"
        public double FertilizerNitrogenPercentage { get; set; } // % N
        public double FertilizerPhosphorusPercentage { get; set; } // % P2O5
        public double FertilizerPotassiumPercentage { get; set; } // % K2O
        public double WiltingPoint { get; set; } // mm (per hectare) - moved from SoilData

        // Navigation property - one-to-many relationship with Crop
        public virtual ICollection<Crop> Crops { get; set; } = new List<Crop>();
    }
}
