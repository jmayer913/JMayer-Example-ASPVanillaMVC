namespace JMayer.Example.ASPVanillaMVC.Models;

/// <summary>
/// The enumeration for type of work order services.
/// </summary>
public enum WorkOrderServiceType
{
    /// <summary>
    /// Insepction of an equipment.
    /// </summary>
    Inspection = 0,

    /// <summary>
    /// Routine maintenance done on an equipment.
    /// </summary>
    Routine,

    /// <summary>
    /// Maintenance done in response to an equipment failure.
    /// </summary>
    Reactive,

    /// <summary>
    /// Any other type of service.
    /// </summary>
    Other
}
