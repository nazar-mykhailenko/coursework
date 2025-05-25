using AgroPlaner.DAL.Repositories.Interfaces;
using AgroPlaner.Services.Interfaces;
using AgroPlaner.Services.Models;
using AgroPlaner.Services.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Linq;

namespace AgroPlaner.Services.Services
{
    public class SoilDataUpdateBackgroundService : BackgroundService
    {
        private readonly ILogger<SoilDataUpdateBackgroundService> _logger;
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly SoilDataUpdateSettings _soilDataUpdateSettings;
        private Timer? _timer;

        public SoilDataUpdateBackgroundService(
            ILogger<SoilDataUpdateBackgroundService> logger,
            IServiceScopeFactory serviceScopeFactory,
            IOptions<SoilDataUpdateSettings> soilDataUpdateSettings
        )
        {
            _logger = logger;
            _serviceScopeFactory = serviceScopeFactory;
            _soilDataUpdateSettings = soilDataUpdateSettings.Value;
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Soil Data Update Service is starting.");

            _timer = new Timer(
                DoWork,
                null,
                TimeSpan.Zero,
                TimeSpan.FromMinutes(_soilDataUpdateSettings.UpdateIntervalMinutes)
            );

            return Task.CompletedTask;
        }
        private async void DoWork(object? state)
        {
            _logger.LogInformation("Soil Data Update Service running at: {time}", DateTimeOffset.Now);

            using (var scope = _serviceScopeFactory.CreateScope())
            {
                var soilDataUpdateService =
                    scope.ServiceProvider.GetRequiredService<ISoilDataUpdateService>();

                // Get services for GDD updates
                var cropRepository = scope.ServiceProvider.GetRequiredService<ICropRepository>();
                var weatherDataService = scope.ServiceProvider.GetRequiredService<IWeatherDataService>();

                try
                {
                    // Update soil data as before
                    await soilDataUpdateService.UpdateAllSoilDataAsync();
                    _logger.LogInformation("Soil data updated successfully");
                    // Update GDD for all crops
                    await UpdateGDDForAllCropsAsync(cropRepository, weatherDataService);
                    _logger.LogInformation("Daily GDD data updated successfully for all crops");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error updating soil data and GDD");
                }
            }
        }
        private async Task UpdateGDDForAllCropsAsync(
            ICropRepository cropRepository,
            IWeatherDataService weatherDataService)
        {
            _logger.LogInformation("Starting daily GDD update for all crops.");

            try
            {
                var crops = await cropRepository.GetAllAsync();
                var updatedCropsCount = 0;

                foreach (var crop in crops)
                {
                    try
                    {
                        // Skip crops without planting date or plant information
                        if (crop.PlantingDate == null || crop.Plant == null)
                        {
                            _logger.LogWarning($"Skipping crop {crop.Id}: missing planting date or plant information");
                            continue;
                        }

                        // Skip crops that haven't been planted yet
                        if (crop.PlantingDate.Value.Date > DateTime.Today)
                        {
                            _logger.LogDebug($"Skipping crop {crop.Id}: planting date {crop.PlantingDate.Value:yyyy-MM-dd} is in the future");
                            continue;
                        }

                        // Get today's weather data for the crop's location
                        var todayWeather = await weatherDataService.GetTodayWeatherForLocationAsync(crop.LocationId);

                        if (todayWeather != null)
                        {
                            // Calculate today's GDD
                            double todayGDD = Math.Max((todayWeather.MaxTemp + todayWeather.MinTemp) / 2 - crop.Plant.BaseTempForGDD, 0);

                            // Add today's GDD to existing cumulative GDD
                            crop.CumulativeGDDToday += todayGDD;

                            // Save the updated crop with new GDD value
                            await cropRepository.UpdateAsync(crop);
                            updatedCropsCount++;

                            _logger.LogDebug($"Added {todayGDD:F2} GDD to crop {crop.Id}. New cumulative GDD: {crop.CumulativeGDDToday:F2}");
                        }
                        else
                        {
                            _logger.LogWarning($"No weather data available for today for crop {crop.Id} at location {crop.LocationId}");
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, $"Error updating GDD for crop {crop.Id}");
                    }
                }

                _logger.LogInformation($"Daily GDD update completed. Updated {updatedCropsCount} crops out of {crops.Count()} total crops.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during daily GDD update process");
                throw;
            }
        }

        public override Task StopAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Soil Data Update Service is stopping.");

            _timer?.Change(Timeout.Infinite, 0);

            return base.StopAsync(stoppingToken);
        }

        public override void Dispose()
        {
            _timer?.Dispose();
            base.Dispose();
        }
    }
}
