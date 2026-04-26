using JMayer.Data.Data;
using JMayer.Data.DataAnnotations;
using System.ComponentModel.DataAnnotations;

namespace JMayer.Example.ASPVanillaMVC.Models;

/// <summary>
/// The class represents a work order template to be used for scheduling.
/// </summary>
public class WorkOrderTemplate : DataObject
{
    /// <summary>
    /// The property gets/sets the number of days the work order will be due from when its created.
    /// </summary>
    /// <remarks>Zero represents no due date.</remarks>
    [Range(0, int.MaxValue)]
    public int DaysDueFromCreation { get; set; }

    /// <inheritdoc/>
    /// <remarks>Overridden to add Required data annotation.</remarks>
    [Required]
    public override string? Name { get => base.Name; set => base.Name = value; }

    /// <summary>
    /// The property gets/sets the user defined type of service when Other is selected for the service.
    /// </summary>
    [RequiredDependsOn(nameof(ServiceType), WorkOrderServiceType.Other)]
    public string? OtherTypeOfService { get; set; }

    /// <summary>
    /// The property gets/sets the priority of the work order.
    /// </summary>
    [Required]
    public WorkOrderPriority Priority { get; set; } = WorkOrderPriority.Normal;

    /// <summary>
    /// The property gets/sets the type of service to be done.
    /// </summary>
    [Required]
    public WorkOrderServiceType ServiceType { get; set; }

    /// <summary>
    /// The default constructor.
    /// </summary>
    public WorkOrderTemplate() { }

    /// <summary>
    /// The copy constructor.
    /// </summary>
    /// <param name="copy">The copy.</param>
    public WorkOrderTemplate(WorkOrderTemplate copy) => MapProperties(copy);

    /// <inheritdoc/>
    /// <remarks>Overridden to map properties for this class.</remarks>
    public override void MapProperties(DataObject dataObject)
    {
        base.MapProperties(dataObject);

        if (dataObject is WorkOrderTemplate workOrderTemplate)
        {
            DaysDueFromCreation = workOrderTemplate.DaysDueFromCreation;
            OtherTypeOfService = workOrderTemplate.OtherTypeOfService;
            Priority = workOrderTemplate.Priority;
            ServiceType = workOrderTemplate.ServiceType;
        }
    }
}
