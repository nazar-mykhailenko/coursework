using AgroPlaner.DAL.Models;
using AgroPlaner.DAL.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace AgroPlaner.DAL.Repositories
{
    public class SoilDataRepository : BaseRepository<SoilData>, ISoilDataRepository
    {
        public SoilDataRepository(DbContext context)
            : base(context) { }
    }
}
