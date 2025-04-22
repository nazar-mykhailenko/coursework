namespace AgroPlaner.Api.Models
{
    public class SoilDataDto
    {
        public int SoilDataId { get; set; }
        public int CropId { get; set; }
        public double CurrentMoisture { get; set; }
        public double FieldCapacity { get; set; }
        public double Temperature { get; set; }
        public double AvailableNitrogen { get; set; }
        public double AvailablePhosphorus { get; set; }
        public double AvailablePotassium { get; set; }
    }

    public class IrrigationEventDto
    {
        public double Amount { get; set; } // mm (per hectare)
    }

    public class FertilizationEventDto
    {
        public double NitrogenAmount { get; set; } // kg/ha
        public double PhosphorusAmount { get; set; } // kg/ha
        public double PotassiumAmount { get; set; } // kg/ha
    }
}
