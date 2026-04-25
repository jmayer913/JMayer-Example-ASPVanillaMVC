using JMayer.Data.Data;
using JMayer.Data.DataAnnotations;

namespace JMayer.Example.ASPVanillaMVC.Models;

/// <summary>
/// The class represents a schedule for a work order template.
/// </summary>
/// <remarks>The owner id will represent the id of the work order template.</remarks>
public class WorkOrderTemplateSchedule : SubDataObject
{
    /// <summary>
    /// The property gets/sets when the scheduler will stop using the schedule.
    /// </summary>
    /// <remarks>Null means the schedule never ends.</remarks>
    [CompareToOtherMember(otherMemberName: nameof(StartDate), compareToOperation: ComparisonOperation.GreaterThanOrEqual, passRegisteredMemberIfNull: true, ErrorMessage = "The End Date must be greater than or equal to the Start Date.")]
    public DateTime? EndDate { get; set; }

    /// <summary>
    /// The property gets/sets if the scheduler uses this schedule.
    /// </summary>
    public bool IsEnabled { get; set; }

    /// <summary>
    /// The property gets/sets the type of schedule to use.
    /// </summary>
    public WorkOrderTemplateScheduleType ScheduleType { get; set; }

    /// <summary>
    /// The property gets/sets when the scheduler will start using the schedule.
    /// </summary>
    public DateTime StartDate { get; set; } = DateTime.Today;

    /// <summary>
    /// The default constructor.
    /// </summary>
    public WorkOrderTemplateSchedule() { }

    /// <summary>
    /// The copy constructor.
    /// </summary>
    /// <param name="copy">The copy.</param>
    public WorkOrderTemplateSchedule(WorkOrderTemplateSchedule copy) => MapProperties(copy);

    /// <inheritdoc/>
    /// <remarks>Overridden to map properties for this class.</remarks>
    public override void MapProperties(DataObject dataObject)
    {
        base.MapProperties(dataObject);

        if (dataObject is WorkOrderTemplateSchedule schedule)
        {
            EndDate = schedule.EndDate;
            IsEnabled = schedule.IsEnabled;
            ScheduleType = schedule.ScheduleType;
            StartDate = schedule.StartDate;
        }
    }
}
