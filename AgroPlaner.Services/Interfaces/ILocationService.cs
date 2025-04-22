using AgroPlaner.DAL.Models;

namespace AgroPlaner.Services.Interfaces
{
    public interface ILocationService
    {
        Task<IEnumerable<Location>> GetAllAsync(string? userId = null);
        Task<Location?> GetByIdAsync(int id, string? userId = null);
        Task<Location> CreateAsync(Location location, string userId);
        Task<Location> UpdateAsync(Location location, string userId);
        Task<bool> DeleteAsync(int id, string userId);
    }
}
