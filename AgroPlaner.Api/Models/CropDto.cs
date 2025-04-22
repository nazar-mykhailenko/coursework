namespace AgroPlaner.Api.Models
{
    public class CropDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public int PlantId { get; set; }
        public int LocationId { get; set; }
        public double ExpectedYield { get; set; } // tons/ha
        public double CumulativeGDDToday { get; set; } // GDD accumulated up to today
        public double FieldArea { get; set; } // ha, area of the field
    }

    public class CreateCropDto
    {
        public string Name { get; set; } = string.Empty;
        public int PlantId { get; set; }
        public int LocationId { get; set; }
        public double ExpectedYield { get; set; }
        public double FieldArea { get; set; }
    }

    public class UpdateCropDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public double ExpectedYield { get; set; }
        public double FieldArea { get; set; }
    }
}
