namespace AgroPlaner.Api.Models
{
    public class PlantDto
    {
        public int PlantId { get; set; }
        public string Name { get; set; } = string.Empty;
        public double MinSoilTempForSeeding { get; set; }
        public double BaseTempForGDD { get; set; }
        public double MaturityGDD { get; set; }
        public double RootDepth { get; set; }
        public double AllowableDepletionFraction { get; set; }
        public double NitrogenContent { get; set; }
        public double PhosphorusContent { get; set; }
        public double PotassiumContent { get; set; }
    }
}
