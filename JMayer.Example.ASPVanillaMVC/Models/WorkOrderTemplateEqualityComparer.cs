using System.Diagnostics.CodeAnalysis;

namespace JMayer.Example.ASPVanillaMVC.Models;

/// <summary>
/// The class manages comparing two WorkOrderTemplate objects.
/// </summary>
public class WorkOrderTemplateEqualityComparer : IEqualityComparer<WorkOrderTemplate>
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
    public WorkOrderTemplateEqualityComparer() { }

    /// <summary>
    /// The property constructor.
    /// </summary>
    /// <param name="excludeCreatedOn">Excludes the CreatedOn property from the equals check.</param>
    /// <param name="exlucdeID">Excludes the ID property from the equals check.</param>
    /// <param name="excludeLastEditedOn">Excludes the LastEditedOn property from the equals check.</param>
    public WorkOrderTemplateEqualityComparer(bool excludeCreatedOn, bool exlucdeID, bool excludeLastEditedOn)
    {
        _excludeCreatedOn = excludeCreatedOn;
        _exlucdeID = exlucdeID;
        _excludeLastEditedOn = excludeLastEditedOn;
    }

    /// <inheritdoc/>
    public bool Equals(WorkOrderTemplate? x, WorkOrderTemplate? y)
    {
        if (x is null || y is null)
        {
            return false;
        }

        return (_excludeCreatedOn || x.CreatedOn == y.CreatedOn)
            && x.DaysDueFromCreation == y.DaysDueFromCreation
            && x.Description == y.Description
            && (_exlucdeID || x.Integer64ID == y.Integer64ID)
            && (_excludeLastEditedOn || x.LastEditedOn == y.LastEditedOn)
            && x.Name == y.Name
            && x.OtherTypeOfService == y.OtherTypeOfService
            && x.Priority == y.Priority
            && x.ServiceType == y.ServiceType;
    }

    /// <inheritdoc/>
    /// <exception cref="NotImplementedException">Thrown because its not expected to be used.</exception>
    public int GetHashCode([DisallowNull] WorkOrderTemplate obj)
    {
        throw new NotImplementedException();
    }
}
