using JMayer.Example.ASPVanillaMVC.DataLayers;
using JMayer.Example.ASPVanillaMVC.Models;

namespace JMayer.Example.ASPVanillaMVC;

/// <summary>
/// The class is used to generate example data for BHS work order templates.
/// </summary>
public class WorkOrderTemplateExampleBuilder
{
    /// <summary>
    /// The property gets/sets the data layer used to interact with work order templates.
    /// </summary>
    public IWorkOrderTemplateDataLayer WorkOrderTemplateDataLayer { get; set; } = new WorkOrderTemplateDataLayer();

    /// <summary>
    /// The property gets/sets the data layer used to interact with work order template schedules.
    /// </summary>
    public IWorkOrderTemplateScheduleDataLayer WorkOrderTemplateScheduleDataLayer { get; set; }

    /// <summary>
    /// The default constructor.
    /// </summary>
    public WorkOrderTemplateExampleBuilder() 
        => WorkOrderTemplateScheduleDataLayer = new WorkOrderTemplateScheduleDataLayer(WorkOrderTemplateDataLayer);

    /// <summary>
    /// The method builds the work order template example data.
    /// </summary>
    public void Build()
    {
        BuildWorkOrderTemplates();
        BuildWorkOrderTemplateSchedules();
    }

    /// <summary>
    /// The method builds the work order templates.
    /// </summary>
    public void BuildWorkOrderTemplates()
    {
        WorkOrderTemplateDataLayer.CreateAsync([
            //Daily
            new WorkOrderTemplate()
            {
                DaysDueFromCreation = 1,
                Description = "Each photoeye needs to be wiped of dust to ensure optimal function.",
                Name = "Photoeye Clean",
                OtherTypeOfService = null,
                Priority = WorkOrderPriority.Normal,
                ServiceType = WorkOrderServiceType.Routine,
            },
            //Weekly
            new WorkOrderTemplate()
            {
                DaysDueFromCreation = 7,
                Description = "Dust and vacuum the control room and remove any trash.",
                Name = "Control Room Clean",
                OtherTypeOfService = null,
                Priority = WorkOrderPriority.Normal,
                ServiceType = WorkOrderServiceType.Routine,
            },
            //Monthly
            new WorkOrderTemplate()
            {
                DaysDueFromCreation = 30,
                Description = "Various electrical tests.",
                Name = "Electrical Inspection",
                OtherTypeOfService = null,
                Priority = WorkOrderPriority.Normal,
                ServiceType = WorkOrderServiceType.Inspection,
            },
            //Quarterly
            new WorkOrderTemplate()
            {
                DaysDueFromCreation = 30,
                Description = "Check the electrical and mechanical components used to raise and lower the conveyor security doors at the ticket counters.",
                Name = "Conveyor Security Door Inspection",
                OtherTypeOfService = null,
                Priority = WorkOrderPriority.Normal,
                ServiceType = WorkOrderServiceType.Inspection,
            },
            //Semiyearly
            new WorkOrderTemplate()
            {
                DaysDueFromCreation = 30,
                Description = "Check the electrical and mechanical components used for the chiller station.",
                Name = "Chiller Station Inspection",
                OtherTypeOfService = null,
                Priority = WorkOrderPriority.Normal,
                ServiceType = WorkOrderServiceType.Inspection,
            },
            //Yearly
            new WorkOrderTemplate()
            {
                DaysDueFromCreation = 30,
                Description = "Check the electrical and mechanical components used to charge the plane's battery while parked at the gate.",
                Name = "Plane Charging Station Inspection",
                OtherTypeOfService = null,
                Priority = WorkOrderPriority.Normal,
                ServiceType = WorkOrderServiceType.Inspection,
            },
        ]);
    }

    /// <summary>
    /// The method builds the schedules for the work order templates.
    /// </summary>
    /// <remarks>A disabled schedule will be automatically created for each template; this enables them and updates the schedule frequency.</remarks>
    public void BuildWorkOrderTemplateSchedules()
    {
        DateTime startOfYear = new(DateTime.Today.Year, 1, 1);
        List<WorkOrderTemplateSchedule> schedules = WorkOrderTemplateScheduleDataLayer.GetAllAsync(orderByPredicate: obj => obj.OwnerInteger64ID).Result;

        //Daily
        schedules[0].ScheduleType = WorkOrderTemplateScheduleType.Daily;
        schedules[0].StartDate = startOfYear;
        schedules[0].IsEnabled = true;

        //Weekly
        schedules[1].ScheduleType = WorkOrderTemplateScheduleType.Weekly;
        schedules[1].StartDate = startOfYear;
        schedules[1].IsEnabled = true;

        //Monthly
        schedules[2].ScheduleType = WorkOrderTemplateScheduleType.Monthly;
        schedules[1].StartDate = startOfYear;
        schedules[2].IsEnabled = true;

        //Quarterly
        schedules[3].ScheduleType = WorkOrderTemplateScheduleType.Quarterly;
        schedules[1].StartDate = startOfYear;
        schedules[3].IsEnabled = true;

        //Semiyearly
        schedules[4].ScheduleType = WorkOrderTemplateScheduleType.Semiyearly;
        schedules[1].StartDate = startOfYear;
        schedules[4].IsEnabled = true;

        //Yearly
        schedules[5].ScheduleType = WorkOrderTemplateScheduleType.Yearly;
        schedules[1].StartDate = startOfYear;
        schedules[5].IsEnabled = true;

        WorkOrderTemplateScheduleDataLayer.UpdateAsync(schedules);
    }
}
