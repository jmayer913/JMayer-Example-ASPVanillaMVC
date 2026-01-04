using JMayer.Data.Database.DataLayer.MemoryStorage;
using JMayer.Example.ASPVanillaMVC.Models;

namespace JMayer.Example.ASPVanillaMVC.DataLayers;

/// <summary>
/// The class manages CRUD interactions with the database for a work order template schedule.
/// </summary>
public class WorkOrderTemplateScheduleDataLayer : StandardSubCRUDDataLayer<WorkOrderTemplateSchedule>, IWorkOrderTemplateScheduleDataLayer
{
    /// <summary>
    /// Used to access the work order template data layer and reac on a create or delete.
    /// </summary>
    private readonly IWorkOrderTemplateDataLayer _workOrderTemplateDataLayer;

    /// <summary>
    /// The depedency injection constructor.
    /// </summary>
    /// <param name="workOrderTemplateDataLayer">Used to access the work order template data layer and reac on a create or delete.</param>
    public WorkOrderTemplateScheduleDataLayer(IWorkOrderTemplateDataLayer workOrderTemplateDataLayer)
    {
        IsOldDataObjectDetectionEnabled = true;
        _workOrderTemplateDataLayer = workOrderTemplateDataLayer;
        _workOrderTemplateDataLayer.Created += WorkOrderTemplateDataLayer_Created;
        _workOrderTemplateDataLayer.Deleted += WorkOrderTemplateDataLayer_Deleted;
    }

    /// <summary>
    /// The method cascade creates a disabled default schedule for the work order template.
    /// </summary>
    /// <param name="sender">The object which called the event.</param>
    /// <param name="e">The event arguments for the event.</param>
    private async void WorkOrderTemplateDataLayer_Created(object? sender, Data.Database.DataLayer.CreatedEventArgs e)
    {
        foreach (var template in e.DataObjects.Cast<WorkOrderTemplate>())
        {
            WorkOrderTemplateSchedule schedule = new()
            {
                OwnerInteger64ID = template.Integer64ID,
            };
            _ = await CreateAsync(schedule);
        }
    }

    /// <summary>
    /// The method cascade deletes any schedules where the parent work order template was deleted.
    /// </summary>
    /// <param name="sender">The object which called the event.</param>
    /// <param name="e">The event arguments for the event.</param>
    private async void WorkOrderTemplateDataLayer_Deleted(object? sender, Data.Database.DataLayer.DeletedEventArgs e)
    {
        foreach (var template in e.DataObjects.Cast<WorkOrderTemplate>())
        {
            WorkOrderTemplateSchedule? schedule = await GetSingleAsync(obj => obj.OwnerInteger64ID == template.Integer64ID);

            if (schedule is not null)
            {
                await DeleteAsync(schedule);
            }
        }
    }
}
