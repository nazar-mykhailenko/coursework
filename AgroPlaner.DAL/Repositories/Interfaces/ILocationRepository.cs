using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using AgroPlaner.DAL.Models;

namespace AgroPlaner.DAL.Repositories.Interfaces
{
    public interface ILocationRepository
    {
        Task<Location> CreateAsync(Location entity);
        Task DeleteAsync(int id);
        Task<IEnumerable<Location>> GetAllAsync();
        Task<Location> GetByIdAsync(int id);
        Task<IEnumerable<Location>> GetFilteredAsync(Expression<Func<Location, bool>> filter);
    }
}
