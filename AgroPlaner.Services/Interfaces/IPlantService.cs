using AgroPlaner.DAL.Models;

namespace AgroPlaner.Services.Interfaces
{
    public interface IPlantService
    {
        Task<IEnumerable<Plant>> GetAllAsync();
        Task<Plant?> GetByIdAsync(int plantId);
    }
}
