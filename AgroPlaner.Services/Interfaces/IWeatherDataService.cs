using AgroPlaner.DAL.Models;

namespace AgroPlaner.Services.Interfaces
{
    public interface IWeatherDataService
    {
        Task<IEnumerable<WeatherData>> GetHistoryByLocationIdAsync(
            int locationId,
            DateTime startDate,
            DateTime endDate
        );
        Task<WeatherData?> GetTodayWeatherForLocationAsync(int locationId);
        Task UpdateWeatherForAllLocationsAsync();
    }
}
