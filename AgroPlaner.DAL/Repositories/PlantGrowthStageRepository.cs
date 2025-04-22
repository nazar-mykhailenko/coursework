using AgroPlaner.DAL.Data;
using AgroPlaner.DAL.Models;
using AgroPlaner.DAL.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace AgroPlaner.DAL.Repositories
{
    public class PlantGrowthStageRepository
        : BaseRepository<PlantGrowthStage>,
            IPlantGrowthStageRepository
    {
        public PlantGrowthStageRepository(ApplicationDbContext context)
            : base(context) { }
    }
}
