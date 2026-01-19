using JMayer.Example.ASPVanillaMVC.DataLayers;
using JMayer.Example.ASPVanillaMVC.Models;

namespace JMayer.Example.ASPVanillaMVC.Services;

/// <summary>
/// The class manages creating the work orders based the templates and their schedules.
/// </summary>
public class WorkOrderSchedulerService : IWorkOrderSchedulerService
{
    /// <summary>
    /// Used to retrieve the current date and time.
    /// </summary>
    private readonly TimeProvider _timeProvider;

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

    /// <summary>
    /// The constant for the month of April.
    /// </summary>
    public const int April = 4;

    /// <summary>
    /// The constant for the month of december.
    /// </summary>
    public const int December = 12;

    /// <inheritdoc/>
    public TimeSpan ExpectedRuntime { get; set; } = TimeSpan.FromHours(0);

    /// <summary>
    /// The constant for the first of the month.
    /// </summary>
    public const int FirstOfMonth = 1;

    /// <summary>
    /// The constant for the month of January.
    /// </summary>
    public const int January = 1;

    /// <summary>
    /// The constant for the month of July.
    /// </summary>
    public const int July = 7;

    /// <inheritdoc/>
    public DateTime LastRanAt { get; set; } = DateTime.Today.AddDays(-1);

    /// <summary>
    /// The constant for the month of October.
    /// </summary>
    public const int October = 10;

    /// <summary>
    /// The dependency injection constructor.
    /// </summary>
    /// <param name="timeProvider">Used to retrieve the current date and time.</param>
    /// <param name="workOrderDataLayer">Used to create the work orders.</param>
    /// <param name="workOrderTemplateDataLayer">Used to query the templates.</param>
    /// <param name="workOrderTemplateScheduleDataLayer">Used to query the schedules.</param>
    public WorkOrderSchedulerService(TimeProvider timeProvider, IWorkOrderDataLayer workOrderDataLayer, IWorkOrderTemplateDataLayer workOrderTemplateDataLayer, IWorkOrderTemplateScheduleDataLayer workOrderTemplateScheduleDataLayer)
    {
        _timeProvider = timeProvider;
        _workOrderDataLayer = workOrderDataLayer;
        _workOrderTemplateDataLayer = workOrderTemplateDataLayer;
        _workOrderTemplateScheduleDataLayer = workOrderTemplateScheduleDataLayer;
    }

    /// <inheritdoc/>
    public bool CanCreateWorkOrders() => _timeProvider.GetLocalNow().Date > LastRanAt && _timeProvider.GetLocalNow().TimeOfDay.CompareTo(ExpectedRuntime) >= 0;

    /// <inheritdoc/>
    public async Task CreateWorkOrdersAsync()
    {
        List<WorkOrderTemplateSchedule> schedules = await _workOrderTemplateScheduleDataLayer.GetAllAsync(obj =>
            obj.IsEnabled
            &&
            (
                obj.ScheduleType == WorkOrderTemplateScheduleType.Daily
                || (obj.ScheduleType == WorkOrderTemplateScheduleType.Weekly && _timeProvider.GetLocalNow().DayOfWeek == DayOfWeek.Monday)
                || (obj.ScheduleType == WorkOrderTemplateScheduleType.Monthly && _timeProvider.GetLocalNow().Day == FirstOfMonth)
                || (obj.ScheduleType == WorkOrderTemplateScheduleType.Quarterly && _timeProvider.GetLocalNow().Day == FirstOfMonth && (_timeProvider.GetLocalNow().Month == January || _timeProvider.GetLocalNow().Month == April || _timeProvider.GetLocalNow().Month == July || _timeProvider.GetLocalNow().Month == October))
                || (obj.ScheduleType == WorkOrderTemplateScheduleType.Semiyearly && _timeProvider.GetLocalNow().Day == FirstOfMonth && (_timeProvider.GetLocalNow().Month == January || _timeProvider.GetLocalNow().Month == July))
                || (obj.ScheduleType == WorkOrderTemplateScheduleType.Yearly && _timeProvider.GetLocalNow().Day == FirstOfMonth && _timeProvider.GetLocalNow().Month == January)
            )
            && _timeProvider.GetLocalNow() >= obj.StartDate 
            && (obj.EndDate == null || _timeProvider.GetLocalNow() <= obj.EndDate)
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
                DueBy = template.DaysDueFromCreation > 0 ? _timeProvider.GetLocalNow().Date.AddDays(template.DaysDueFromCreation) : null,
                Name = $"{template.Name} {_timeProvider.GetLocalNow().LocalDateTime.ToShortDateString()}",
                OtherTypeOfService = template.OtherTypeOfService,
                Priority = template.Priority,
                ServiceType = template.ServiceType,
            });
        }
    }
}
