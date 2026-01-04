using JMayer.Example.ASPVanillaMVC.DataLayers;
using JMayer.Example.ASPVanillaMVC.Models;

namespace JMayer.Example.ASPVanillaMVC.Services;

/// <summary>
/// The class manages creating the work orders based the templates and their schedules.
/// </summary>
public class WorkOrderSchedulerService : IWorkOrderSchedulerService
{
    /// <summary>
    /// Used to create the work orders.
    /// </summary>
    private readonly IWorkOrderDataLayer _workOrderDataLayer;

    /// <summary>
    /// Used to query the templates.
    /// </summary>
    private readonly IWorkOrderTemplateDataLayer _workOrderTemplateDataLayer;

    /// <summary>
    /// Used to query the schedules.
    /// </summary>
    private readonly IWorkOrderTemplateScheduleDataLayer _workOrderTemplateScheduleDataLayer;

    /// <inheritdoc/>
    public TimeSpan ExpectedRuntime { get; set; } = TimeSpan.FromHours(0);

    /// <inheritdoc/>
    public DateTime LastRanAt { get; set; } = DateTime.Today.AddDays(-1);

    /// <summary>
    /// The dependency injection constructor.
    /// </summary>
    /// <param name="workOrderDataLayer">Used to create the work orders.</param>
    /// <param name="workOrderTemplateDataLayer">Used to query the templates.</param>
    /// <param name="workOrderTemplateScheduleDataLayer">Used to query the schedules.</param>
    public WorkOrderSchedulerService(IWorkOrderDataLayer workOrderDataLayer, IWorkOrderTemplateDataLayer workOrderTemplateDataLayer, IWorkOrderTemplateScheduleDataLayer workOrderTemplateScheduleDataLayer)
    {
        _workOrderDataLayer = workOrderDataLayer;
        _workOrderTemplateDataLayer = workOrderTemplateDataLayer;
        _workOrderTemplateScheduleDataLayer = workOrderTemplateScheduleDataLayer;
    }

    /// <inheritdoc/>
    public bool CanCreateWorkOrders() => DateTime.Today > LastRanAt && DateTime.Now.TimeOfDay.CompareTo(ExpectedRuntime) >= 0;

    /// <inheritdoc/>
    public async Task CreateWorkOrdersAsync()
    {
        List<WorkOrderTemplateSchedule> schedules = await _workOrderTemplateScheduleDataLayer.GetAllAsync(obj =>
            obj.IsEnabled
            &&
            (
                obj.ScheduleType == WorkOrderTemplateScheduleType.Daily
                || (obj.ScheduleType == WorkOrderTemplateScheduleType.Weekly && DateTime.Today.DayOfWeek == DayOfWeek.Monday)
                || (obj.ScheduleType == WorkOrderTemplateScheduleType.Monthly && DateTime.Today.Day == 1)
                || (obj.ScheduleType == WorkOrderTemplateScheduleType.Quarterly && DateTime.Today.Day == 1 && (DateTime.Today.Month == 1 || DateTime.Today.Month == 4 || DateTime.Today.Month == 7 || DateTime.Today.Month == 10))
                || (obj.ScheduleType == WorkOrderTemplateScheduleType.Semiyearly && DateTime.Today.Day == 1 && (DateTime.Today.Month == 1 || DateTime.Today.Month == 7))
                || (obj.ScheduleType == WorkOrderTemplateScheduleType.Yearly && DateTime.Today.Day == 1 && DateTime.Today.Month == 1)
            )
            && DateTime.Today >= obj.StartDate 
            && (obj.EndDate == null || DateTime.Today <= obj.EndDate)
        );

        foreach (var schedule in schedules) 
        {
            WorkOrderTemplate? template = await _workOrderTemplateDataLayer.GetSingleAsync(obj => obj.Integer64ID == schedule.OwnerInteger64ID);

            if (template is null)
            {
                continue;
            }

            _ = await _workOrderDataLayer.CreateAsync(new WorkOrder()
            {
                Description = template.Description,
                DueBy = template.DaysDueFromCreation > 0 ? DateTime.Today.AddDays(template.DaysDueFromCreation) : null,
                Name = $"{template.Name} {DateTime.Today.ToShortDateString()}",
                OtherTypeOfService = template.OtherTypeOfService,
                Priority = template.Priority,
                ServiceType = template.ServiceType,
            });
        }
    }
}
