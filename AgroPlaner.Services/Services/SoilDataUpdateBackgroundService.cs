using AgroPlaner.Services.Interfaces;
using AgroPlaner.Services.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

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
                try
                {
                    await soilDataUpdateService.UpdateAllSoilDataAsync();
                    _logger.LogInformation("Soil data updated successfully");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error updating soil data");
                }
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
