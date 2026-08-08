using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using TaskManager.Application.Interfaces;

namespace TaskManager.Infrastructure
{
    public class SoftDeletePurgeWorker : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<SoftDeletePurgeWorker> _logger;

        public SoftDeletePurgeWorker(
            IServiceProvider serviceProvider,
            ILogger<SoftDeletePurgeWorker> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("SoftDeletePurgeWorker initialized.");

            // Perform an initial purge on startup immediately
            await TriggerPurgeAsync(stoppingToken);

            // PeriodicTimer prevents scheduling drift and is thread-safe
            using var timer = new PeriodicTimer(TimeSpan.FromHours(24));

            try
            {
                while (await timer.WaitForNextTickAsync(stoppingToken))
                {
                    await TriggerPurgeAsync(stoppingToken);
                }
            }
            catch (OperationCanceledException)
            {
                _logger.LogInformation("SoftDeletePurgeWorker is stopping due to cancellation.");
            }
        }

        private async Task TriggerPurgeAsync(CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation("Starting soft-deleted tasks purge operation...");

                using (var scope = _serviceProvider.CreateScope())
                {
                    var taskService = scope.ServiceProvider.GetRequiredService<ITaskService>();
                    await taskService.PurgeSoftDeletedTasksAsync(cancellationToken);
                }

                _logger.LogInformation("Soft-deleted tasks purge operation completed successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while purging soft-deleted tasks.");
            }
        }
    }
}