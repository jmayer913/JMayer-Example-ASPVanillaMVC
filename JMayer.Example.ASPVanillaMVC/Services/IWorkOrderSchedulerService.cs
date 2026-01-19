namespace JMayer.Example.ASPVanillaMVC.Services;

/// <summary>
/// The interface for interacting with the work order scheduler.
/// </summary>
public interface IWorkOrderSchedulerService
{
    /// <summary>
    /// The property gets/sets the expected runtime for the scheduler.
    /// </summary>
    /// <remarks>Used when determining if the scheduler can run.</remarks>
    TimeSpan ExpectedRuntime { get; set; }

    /// <summary>
    /// The porperty gets/sets when the scheduler last ran.
    /// </summary>
    /// <remarks>Used when determining if the scheduler can run.</remarks>
    DateTime LastRanAt { get; set; }

    /// <summary>
    /// The method returns if work orders can be created.
    /// </summary>
    /// <returns>True means work orders can be created by the scheduler; false means they can't be.</returns>
    bool CanCreateWorkOrders();

    /// <summary>
    /// The method creates the work orders based on the templates and their schedules.
    /// </summary>
    /// <returns>A Task for the async.</returns>
    Task CreateWorkOrdersAsync();
}
