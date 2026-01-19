namespace JMayer.Example.ASPVanillaMVC.Models;

/// <summary>
/// The enumeration for the type of schedules.
/// </summary>
public enum WorkOrderTemplateScheduleType
{
    /// <summary>
    /// The scheduler creates the work order each day.
    /// </summary>
    Daily,

    /// <summary>
    /// The scheduler creates the work order each week on Monday.
    /// </summary>
    Weekly,

    /// <summary>
    /// The scheduler creates the work order each month on the first.
    /// </summary>
    Monthly,

    /// <summary>
    /// The scheduler creates the work order four times a year, first of January, first of April, first of July and first of October.
    /// </summary>
    Quarterly,

    /// <summary>
    /// The scheduler creates the work order twice a year, first of January and first of June.
    /// </summary>
    Semiyearly,

    /// <summary>
    /// The scheduler creates the work order each year on the first of January.
    /// </summary>
    Yearly
}
