using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using AgroPlaner.DAL.Models;

namespace AgroPlaner.DAL.Repositories.Interfaces
{
    public interface ICropRepository : IRepository<Crop>
    {
        Task<(IEnumerable<Crop> Items, int TotalCount)> GetPagedAsync(
            int pageNumber,
            int pageSize,
            Expression<Func<Crop, bool>> filter = null
        );
        Task<IEnumerable<Crop>> GetFilteredAsync(Expression<Func<Crop, bool>> filter);
    }
}
