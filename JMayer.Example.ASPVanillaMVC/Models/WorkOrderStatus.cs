namespace JMayer.Example.ASPVanillaMVC.Models;

/// <summary>
/// The enumeration for the statuses of the work order.
/// </summary>
public enum WorkOrderStatus
{
    /// <summary>
    /// No one has started the work order.
    /// </summary>
    Open = 0,

    /// <summary>
    /// Someone is working on the work order.
    /// </summary>
    InProgress,

    /// <summary>
    /// The work has finished and it needs a final signoff.
    /// </summary>
    Resolved,

    /// <summary>
    /// The work order is no longer active.
    /// </summary>
    Closed
}
