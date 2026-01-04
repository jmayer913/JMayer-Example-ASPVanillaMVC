using JMayer.Data.Data;
using System.ComponentModel.DataAnnotations;

namespace JMayer.Example.ASPVanillaMVC.Models;

/// <summary>
/// The class represents a work order.
/// </summary>
public class WorkOrder : DataObject
{
    /// <summary>
    /// The property gets/sets when the work order is expected to done by.
    /// </summary>
    public DateTime? DueBy { get; set; }

    /// <inheritdoc/>
    /// <remarks>Overridden to add Required data annotation.</remarks>
    [Required]
    public override string? Name { get => base.Name; set => base.Name = value; }

    /// <summary>
    /// The property gets/sets the user defined type of service when Other is selected for the service.
    /// </summary>
    public string? OtherTypeOfService { get; set; }

    /// <summary>
    /// The property gets/sets the priority of the work order.
    /// </summary>
    [Required]
    public WorkOrderPriority Priority { get; set; } = WorkOrderPriority.Normal;

    /// <summary>
    /// The property gets/sets the problem found.
    /// </summary>
    public string? Problem { get; set; } = string.Empty;

    /// <summary>
    /// The property gets/sets the resolution to the work order.
    /// </summary>
    public string? Resolution { get; set; } = string.Empty;

    /// <summary>
    /// The property gets/sets the type of service to be done.
    /// </summary>
    [Required]
    public WorkOrderServiceType ServiceType { get; set; }

    /// <summary>
    /// The property get/sets the status of the work order.
    /// </summary>
    [Required]
    public WorkOrderStatus Status { get; set; }

    /// <summary>
    /// The default constructor.
    /// </summary>
    public WorkOrder() { }

    /// <summary>
    /// The copy constructor.
    /// </summary>
    /// <param name="copy">The copy.</param>
    public WorkOrder(WorkOrder copy) => MapProperties(copy);

    /// <inheritdoc/>
    public override void MapProperties(DataObject dataObject)
    {
        base.MapProperties(dataObject);

        if (dataObject is WorkOrder workOrder)
        {
            DueBy = workOrder.DueBy;
            OtherTypeOfService = workOrder.OtherTypeOfService;
            Priority = workOrder.Priority;
            Problem = workOrder.Problem;
            Resolution = workOrder.Resolution;
            ServiceType = workOrder.ServiceType;
            Status = workOrder.Status;
        }
    }
}
