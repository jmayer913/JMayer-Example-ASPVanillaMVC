using JMayer.Example.ASPVanillaMVC.Services;

namespace JMayer.Example.ASPVanillaMVC.Workers;

/// <summary>
/// The class manages creating work orders based on the templates and their schedules each day at 12AM.
/// </summary>
public class WorkOrderSchedulerWorker : BackgroundService
{
    /// <summary>
    /// Used to log activity in the worker.
    /// </summary>
    private readonly ILogger<WorkOrderSchedulerWorker> _logger;

    /// <summary>
    /// Used to create work orders based on the templates and their schedules.
    /// </summary>
    private readonly IWorkOrderSchedulerService _workOrderScheduler;

    /// <summary>
    /// The dependency injection constructor.
    /// </summary>
    /// <param name="workOrderScheduler">Used to create work orders based on the templates and their schedules.</param>
    public WorkOrderSchedulerWorker(ILogger<WorkOrderSchedulerWorker> logger, IWorkOrderSchedulerService workOrderScheduler)
    {
        _logger = logger;
        _workOrderScheduler = workOrderScheduler;
    }

    /// <inheritdoc/>
    /// <remarks>Overridden to create work orders each day on the expected runtime.</remarks>
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (stoppingToken.IsCancellationRequested is false)
        {
            //Run each day at 12AM.
            if (_workOrderScheduler.CanCreateWorkOrders())
            {
                _logger.LogInformation("The work order scheduler will do its daily run.");

                try
                {
                    await _workOrderScheduler.CreateWorkOrdersAsync();
                    _workOrderScheduler.LastRanAt = DateTime.Now;
                    _logger.LogInformation("The work order scheduler successfully did its daily run.");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "A failure occurred while the work order scheduler was doing its daily run.");
                }
            }

            try
            {
                await Task.Delay(60_000, stoppingToken);
            }
            catch (Exception) { }
        }
    }
}
