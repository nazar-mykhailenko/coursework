using AgroPlaner.Services.Interfaces;
using AgroPlaner.Services.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace AgroPlaner.Services.Services
{
    public class WeatherUpdateBackgroundService : BackgroundService
    {
        private readonly ILogger<WeatherUpdateBackgroundService> _logger;
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly WeatherApiSettings _weatherApiSettings;
        private Timer? _timer;

        public WeatherUpdateBackgroundService(
            ILogger<WeatherUpdateBackgroundService> logger,
            IServiceScopeFactory serviceScopeFactory,
            IOptions<WeatherApiSettings> weatherApiSettings
        )
        {
            _logger = logger;
            _serviceScopeFactory = serviceScopeFactory;
            _weatherApiSettings = weatherApiSettings.Value;
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Weather Update Service is starting.");

            _timer = new Timer(
                DoWork,
                null,
                TimeSpan.Zero,
                TimeSpan.FromMinutes(_weatherApiSettings.CacheDurationMinutes)
            );

            return Task.CompletedTask;
        }

        private async void DoWork(object? state)
        {
            _logger.LogInformation("Weather Update Service running at: {time}", DateTimeOffset.Now);

            using (var scope = _serviceScopeFactory.CreateScope())
            {
                var weatherService =
                    scope.ServiceProvider.GetRequiredService<IWeatherDataService>();
                try
                {
                    await weatherService.UpdateWeatherForAllLocationsAsync();
                    _logger.LogInformation("Weather data updated successfully");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error updating weather data");
                }
            }
        }

        public override Task StopAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Weather Update Service is stopping.");

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
