using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using AgroPlaner.DAL.Data;
using AgroPlaner.DAL.Models;
using AgroPlaner.DAL.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace AgroPlaner.DAL.Repositories
{
    public class CropRepository : BaseRepository<Crop>, ICropRepository
    {
        public CropRepository(ApplicationDbContext context)
            : base(context) { }

        public async Task<IEnumerable<Crop>> GetFilteredAsync(Expression<Func<Crop, bool>> filter)
        {
            return await _dbSet.Where(filter).ToListAsync();
        }

        public async Task<(IEnumerable<Crop> Items, int TotalCount)> GetPagedAsync(
            int pageNumber,
            int pageSize,
            Expression<Func<Crop, bool>> filter = null
        )
        {
            var query = filter != null ? _dbSet.Where(filter) : _dbSet;

            var totalCount = await query.CountAsync();
            var items = await query.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync();

            return (items, totalCount);
        }
    }
}
