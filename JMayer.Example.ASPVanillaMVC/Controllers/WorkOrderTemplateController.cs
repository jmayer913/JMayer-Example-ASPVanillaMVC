using JMayer.Example.ASPVanillaMVC.DataLayers;
using JMayer.Example.ASPVanillaMVC.Models;
using JMayer.Web.Mvc.Controller.Mvc;
using Microsoft.AspNetCore.Mvc;

namespace JMayer.Example.ASPVanillaMVC.Controllers;

/// <summary>
/// The class manages HTTP requests for views and actions associated with work order templates.
/// </summary>
public class WorkOrderTemplateController : StandardModelViewController<WorkOrderTemplate, IWorkOrderTemplateDataLayer>
{
    /// <summary>
    /// Used to interact with the work order collection.
    /// </summary>
    private readonly IWorkOrderDataLayer _workOrderDataLayer;

    /// <inheritdoc/>
    public WorkOrderTemplateController(IWorkOrderTemplateDataLayer dataLayer, ILogger<WorkOrderTemplateController> logger, IWorkOrderDataLayer workOrderDataLayer) : base(dataLayer, logger) 
        => _workOrderDataLayer = workOrderDataLayer;

    /// <summary>
    /// The method returns the confirm create work order view.
    /// </summary>
    /// <param name="id">The id for the record.</param>
    /// <returns>The confirm create work order view or a negative status code.</returns>
    public async Task<IActionResult> ConfirmCreateWorkOrderViewAsync(long id)
    {
        try
        {
            Logger.LogInformation("Attempting to retrieve the {Type} data object for {ID} for the Confirm Create Work Order View.", DataObjectTypeName, id);

            WorkOrderTemplate? workOrderTemplate = await DataLayer.GetSingleAsync(obj => obj.Integer64ID == id);

            if (workOrderTemplate is null)
            {
                Logger.LogError("The {Type} data object for {ID} was not found so the Confirm Create Work Order View could not be returned.", DataObjectTypeName, id);
                return NotFound();
            }

            Logger.LogInformation("The {Type} data object for {ID} for the Confirm Create Work Order View was successfully retrieved; returning the view.", DataObjectTypeName, id);
            return View($"WorkOrderTemplateConfirmCreateWorkOrder", workOrderTemplate);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Failed to return the Confirm Create Work Order View for the {Type} data object for {ID}.", DataObjectTypeName, id);
            return Problem();
        }
    }

    /// <summary>
    /// The method creates a work order from the template.
    /// </summary>
    /// <param name="id">The id for the record.</param>
    /// <returns>The work order index view or a negative status code.</returns>
    public async Task<IActionResult> CreateWorkOrderAsync(long id)
    {
        try
        {
            Logger.LogInformation("Attempting to create a work order from the {ID} template.", id);

            WorkOrderTemplate? workOrderTemplate = await DataLayer.GetSingleAsync(obj => obj.Integer64ID == id);

            if (workOrderTemplate is null)
            {
                Logger.LogError("The {Type} data object for {ID} was not found so the work order could not be created.", DataObjectTypeName, id);
                return NotFound();
            }

            _ = _workOrderDataLayer.CreateAsync(new WorkOrder()
            {
                Description = workOrderTemplate.Description,
                DueBy = workOrderTemplate.DaysDueFromCreation > 0 ? DateTime.Today.AddDays(workOrderTemplate.DaysDueFromCreation) : null,
                Name = $"{workOrderTemplate.Name} {DateTime.Today.ToShortDateString()}",
                OtherTypeOfService = workOrderTemplate.OtherTypeOfService,
                Priority = workOrderTemplate.Priority,
                ServiceType = workOrderTemplate.ServiceType,
            });

            Logger.LogInformation("The work order for the {ID} template was successfully created; returning the work order index view.", id);

            return RedirectToAction(nameof(Index), nameof(WorkOrder));
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Failed to create the work order from the {ID} template.", id);
            return Problem();
        }
    }
}
