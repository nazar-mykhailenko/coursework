using AgroPlaner.DAL.Data;
using AgroPlaner.DAL.Models;
using AgroPlaner.DAL.Repositories;
using AgroPlaner.DAL.Repositories.Interfaces;
using AgroPlaner.Services.Interfaces;
using AgroPlaner.Services.Models;
using AgroPlaner.Services.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace AgroPlaner.Services.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddAgroPlanerServices(
            this IServiceCollection services,
            IConfiguration configuration
        )
        {
            // Register repositories
            services.AddScoped<ICropRepository, CropRepository>();
            services.AddScoped<IPlantRepository, PlantRepository>();
            services.AddScoped<ISoilDataRepository, SoilDataRepository>();
            services.AddScoped<ILocationRepository, LocationRepository>();
            services.AddScoped<IPlantGrowthStageRepository, PlantGrowthStageRepository>();
            services.AddScoped<IWeatherDataRepository, WeatherDataRepository>();
            services.AddScoped<IEventRepository<IrrigationEvent>, IrrigationEventRepository>();
            services.AddScoped<
                IEventRepository<FertilizationEvent>,
                FertilizationEventRepository
            >();

            // Register services
            services.AddScoped<ICropService, CropService>();
            services.AddScoped<IPlantService, PlantService>();
            services.AddScoped<ISoilDataService, SoilDataService>();
            services.AddScoped<ILocationService, LocationService>();
            services.AddScoped<IWeatherDataService, WeatherDataService>();
            services.AddScoped<AgriPredictionService>();
            services.AddScoped<IPredictionsHelperService, PredictionsHelperService>();
            services.AddScoped<ISoilDataUpdateService, SoilDataUpdateService>();

            // Register background services
            services.AddHostedService<WeatherUpdateBackgroundService>();

            // Configure HttpClient for weather API
            services.AddHttpClient();

            // Register WeatherApiSettings from configuration
            services.Configure<WeatherApiSettings>(configuration.GetSection("WeatherApiSettings"));

            return services;
        }
    }
}
