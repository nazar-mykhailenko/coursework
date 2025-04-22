using AgroPlaner.DAL.Models;

namespace AgroPlaner.Services.Interfaces
{
    public interface ICropService
    {
        Task<IEnumerable<Crop>> GetAllAsync(string? userId = null);
        Task<Crop?> GetByIdAsync(int id, string? userId = null);
        Task<Crop> CreateAsync(Crop crop, string userId);
        Task<Crop> UpdateAsync(Crop crop, string userId);
        Task<bool> DeleteAsync(int id, string userId);
        Task<(IEnumerable<Crop> Items, int TotalCount)> GetPagedAsync(
            int pageNumber,
            int pageSize,
            string? userId = null
        );
    }
}
