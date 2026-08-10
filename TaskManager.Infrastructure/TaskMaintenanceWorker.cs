using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using TaskManager.Application.Interfaces;

namespace TaskManager.Infrastructure
{
    public class TaskMaintenanceWorker : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<TaskMaintenanceWorker> _logger;

        public TaskMaintenanceWorker(
            IServiceProvider serviceProvider,
            ILogger<TaskMaintenanceWorker> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("TaskMaintenanceWorker initialized.");

            // Perform initial maintenance tasks immediately on startup
            await RunMaintenanceTasksAsync(stoppingToken);

            // PeriodicTimer prevents scheduling drift and is thread-safe
            using var timer = new PeriodicTimer(TimeSpan.FromHours(24));

            try
            {
                while (await timer.WaitForNextTickAsync(stoppingToken))
                {
                    await RunMaintenanceTasksAsync(stoppingToken);
                }
            }
            catch (OperationCanceledException)
            {
                _logger.LogInformation("TaskMaintenanceWorker is stopping due to cancellation.");
            }
        }

        private async Task RunMaintenanceTasksAsync(CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation("Starting periodic task maintenance operations...");

                using (var scope = _serviceProvider.CreateScope())
                {
                    var taskService = scope.ServiceProvider.GetRequiredService<ITaskService>();

                    // 1. Purge soft-deleted tasks older than 30 days
                    _logger.LogInformation("Purging soft-deleted tasks...");
                    await taskService.PurgeSoftDeletedTasksAsync(cancellationToken);

                    // 2. Archive completed tasks older than 30 days
                    _logger.LogInformation("Archiving completed tasks...");
                    await taskService.ArchiveCompletedTasksAsync(cancellationToken);
                }

                _logger.LogInformation("Periodic task maintenance operations completed successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred during periodic task maintenance.");
            }
        }
    }
}
