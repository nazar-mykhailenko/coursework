using AgroPlaner.DAL.Models;
using AgroPlaner.DAL.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace AgroPlaner.DAL.Repositories
{
    public class FertilizationEventRepository
        : EventRepository<FertilizationEvent>,
            IEventRepository<FertilizationEvent>
    {
        public FertilizationEventRepository(DbContext context)
            : base(context) { }
    }
}
