using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AgroPlaner.DAL.Models
{
    public class PlantGrowthStage
    {
        [Key]
        public int PlantGrowthStageId { get; set; }

        [Required]
        public string StageName { get; set; }

        public double MinGDD { get; set; }
        public double MaxGDD { get; set; }

        public int OrderIndex { get; set; }

        public int PlantId { get; set; }

        [ForeignKey("PlantId")]
        public virtual Plant Plant { get; set; }
    }
}
