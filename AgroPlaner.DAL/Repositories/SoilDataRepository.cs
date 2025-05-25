using AgroPlaner.DAL.Data;
using AgroPlaner.DAL.Models;
using AgroPlaner.DAL.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace AgroPlaner.DAL.Repositories
{
    public class SoilDataRepository : BaseRepository<SoilData>, ISoilDataRepository
    {
        public SoilDataRepository(ApplicationDbContext context)
            : base(context) { }

        public override async Task<SoilData> UpdateAsync(SoilData entity)
        {
            // Check if entity is already being tracked
            var trackedEntity = _context.Entry(entity);

            if (trackedEntity.State == EntityState.Detached)
            {
                // Entity is not tracked, check if another entity with same key is tracked
                var existingEntity = await _context.SoilData
                    .AsNoTracking()
                    .FirstOrDefaultAsync(s => s.SoilDataId == entity.SoilDataId);

                if (existingEntity != null)
                {
                    // Update the existing tracked entity if it exists
                    var tracked = _context.SoilData.Local
                        .FirstOrDefault(s => s.SoilDataId == entity.SoilDataId);

                    if (tracked != null)
                    {
                        // Update properties of tracked entity
                        _context.Entry(tracked).CurrentValues.SetValues(entity);
                        await _context.SaveChangesAsync();
                        return tracked;
                    }
                    else
                    {
                        // Attach and update
                        _context.SoilData.Attach(entity);
                        _context.Entry(entity).State = EntityState.Modified;
                    }
                }
                else
                {
                    _dbSet.Update(entity);
                }
            }

            await _context.SaveChangesAsync();
            return entity;
        }
    }
}
