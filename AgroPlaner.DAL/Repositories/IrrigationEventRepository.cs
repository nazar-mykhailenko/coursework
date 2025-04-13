using AgroPlaner.DAL.Models;
using AgroPlaner.DAL.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace AgroPlaner.DAL.Repositories
{
    public class IrrigationEventRepository
        : EventRepository<IrrigationEvent>,
            IEventRepository<IrrigationEvent>
    {
        public IrrigationEventRepository(DbContext context)
            : base(context) { }
    }
}
