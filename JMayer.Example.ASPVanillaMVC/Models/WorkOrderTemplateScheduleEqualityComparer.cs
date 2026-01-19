using System.Diagnostics.CodeAnalysis;

namespace JMayer.Example.ASPVanillaMVC.Models;

/// <summary>
/// The class manages comparing two WorkOrderTemplateSchedule objects.
/// </summary>
public class WorkOrderTemplateScheduleEqualityComparer : IEqualityComparer<WorkOrderTemplateSchedule>
{
    /// <summary>
    /// Excludes the CreatedOn property from the equals check.
    /// </summary>
    private readonly bool _excludeCreatedOn;

    /// <summary>
    /// Excludes the ID property from the equals check.
    /// </summary>
    private readonly bool _exlucdeID;

    /// <summary>
    /// Excludes the LastEditedOn property from the equals check.
    /// </summary>
    private readonly bool _excludeLastEditedOn;

    /// <summary>
    /// The default constructor.
    /// </summary>
    public WorkOrderTemplateScheduleEqualityComparer() { }

    /// <summary>
    /// The property constructor.
    /// </summary>
    /// <param name="excludeCreatedOn">Excludes the CreatedOn property from the equals check.</param>
    /// <param name="exlucdeID">Excludes the ID property from the equals check.</param>
    /// <param name="excludeLastEditedOn">Excludes the LastEditedOn property from the equals check.</param>
    public WorkOrderTemplateScheduleEqualityComparer(bool excludeCreatedOn, bool exlucdeID, bool excludeLastEditedOn)
    {
        _excludeCreatedOn = excludeCreatedOn;
        _exlucdeID = exlucdeID;
        _excludeLastEditedOn = excludeLastEditedOn;
    }

    /// <inheritdoc/>
    public bool Equals(WorkOrderTemplateSchedule? x, WorkOrderTemplateSchedule? y)
    {
        if (x is null || y is null)
        {
            return false;
        }

        return (_excludeCreatedOn || x.CreatedOn == y.CreatedOn)
            && x.Description == y.Description
            && x.EndDate == y.EndDate
            && (_exlucdeID || x.Integer64ID == y.Integer64ID)
            && x.IsEnabled == y.IsEnabled
            && (_excludeLastEditedOn || x.LastEditedOn == y.LastEditedOn)
            && x.Name == y.Name
            && x.ScheduleType == y.ScheduleType
            && x.StartDate == y.StartDate;
    }

    /// <inheritdoc/>
    /// <exception cref="NotImplementedException">Thrown because its not expected to be used.</exception>
    public int GetHashCode([DisallowNull] WorkOrderTemplateSchedule obj)
    {
        throw new NotImplementedException();
    }
}
