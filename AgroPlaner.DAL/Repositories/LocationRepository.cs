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
    public class LocationRepository : ILocationRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly DbSet<Location> _dbSet;

        public LocationRepository(ApplicationDbContext context)
        {
            _context = context;
            _dbSet = context.Set<Location>();
        }

        public async Task<Location> CreateAsync(Location entity)
        {
            await _dbSet.AddAsync(entity);
            await _context.SaveChangesAsync();
            return entity;
        }

        public async Task DeleteAsync(int id)
        {
            var entity = await _dbSet.FindAsync(id);
            if (entity != null)
            {
                _dbSet.Remove(entity);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<IEnumerable<Location>> GetAllAsync()
        {
            return await _dbSet.ToListAsync();
        }

        public async Task<Location> GetByIdAsync(int id)
        {
            return await _dbSet.FindAsync(id);
        }

        public async Task<IEnumerable<Location>> GetFilteredAsync(
            Expression<Func<Location, bool>> filter
        )
        {
            return await _dbSet.Where(filter).ToListAsync();
        }
    }
}
