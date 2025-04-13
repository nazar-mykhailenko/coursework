using AgroPlaner.DAL.Models;
using AgroPlaner.DAL.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace AgroPlaner.DAL.Repositories
{
    public class PlantRepository : BaseRepository<Plant>, IPlantRepository
    {
        public PlantRepository(DbContext context)
            : base(context) { }
    }
}
