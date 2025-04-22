using AgroPlaner.DAL.Data;
using AgroPlaner.DAL.Models;
using AgroPlaner.DAL.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace AgroPlaner.DAL.Repositories
{
    public class WeatherDataRepository : BaseRepository<WeatherData>, IWeatherDataRepository
    {
        public WeatherDataRepository(ApplicationDbContext context)
            : base(context) { }
    }
}
