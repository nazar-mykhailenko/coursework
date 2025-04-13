using System.Collections.Generic;
using System.Threading.Tasks;

namespace AgroPlaner.DAL.Repositories.Interfaces
{
    public interface IEventRepository<T>
        where T : class
    {
        Task<T> CreateAsync(T entity);
        Task<IEnumerable<T>> GetAllAsync();
        Task<T> GetByIdAsync(int id);
    }
}
