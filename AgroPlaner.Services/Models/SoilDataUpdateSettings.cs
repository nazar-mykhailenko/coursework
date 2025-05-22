namespace AgroPlaner.Services.Models
{
    public class SoilDataUpdateSettings
    {
        public int UpdateIntervalMinutes { get; set; } = 240; // Default to 4 hours if not specified
    }
}
