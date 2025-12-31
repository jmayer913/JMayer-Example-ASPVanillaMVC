using JMayer.Data.HTTP.Details;
using JMayer.Example.ASPVanillaMVC.DataLayers;
using JMayer.Example.ASPVanillaMVC.Models;
using JMayer.Web.Mvc.Controller.Mvc;
using JMayer.Web.Mvc.Extension;
using Microsoft.AspNetCore.Mvc;

namespace JMayer.Example.ASPVanillaMVC.Controllers;

#warning I think I'm only going to allow editing so I should add the Nonaction attribute to the actions I don't intend to use.

/// <summary>
/// The class manages HTTP requests for views and actions associated with work order template schedules.
/// </summary>
public class WorkOrderTemplateScheduleController : StandardSubModelViewController<WorkOrderTemplateSchedule, IWorkOrderTemplateScheduleDataLayer>
{
    /// <inheritdoc/>
    public WorkOrderTemplateScheduleController(IWorkOrderTemplateScheduleDataLayer dataLayer, ILogger<WorkOrderTemplateScheduleController> logger) : base(dataLayer, logger) { }

    /// <inheritdoc/>
    /// <remarks>Overridden to query using OwnerInteger64ID instead of Integer64ID.</remarks>
    public override async Task<IActionResult> EditViewAsync(long id)
    {
        try
        {
            Logger.LogInformation("Attempting to retrieve the {Type} data object for {ID} for the Edit View.", DataObjectTypeName, id);

            WorkOrderTemplateSchedule? dataObject = await DataLayer.GetSingleAsync(obj => obj.OwnerInteger64ID == id);

            if (dataObject is null)
            {
                Logger.LogError("The {Type} data object for {ID} was not found so the Edit View could not be returned.", DataObjectTypeName, id);
                return IsDetailsIncludedInNegativeResponse ? NotFound(new NotFoundDetails(title: $"{DataObjectTypeName.SpaceCapitalLetters()} Edit View Error - Not Found", detail: $"The {DataObjectTypeName.SpaceCapitalLetters()} record was not found; please refresh the page because another user may have deleted it.")) : NotFound();
            }

            Logger.LogInformation("The {Type} data object for {ID} for the Edit View was successfully retrieved; returning the view.", DataObjectTypeName, id);
            return View($"{DataObjectTypeName}Edit", dataObject);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Failed to return the Edit View for the {Type} data object for {ID}.", DataObjectTypeName, id);
            return IsDetailsIncludedInNegativeResponse ? Problem(title: $"{DataObjectTypeName.SpaceCapitalLetters()} Edit View Error", detail: $"Failed to find the {DataObjectTypeName.SpaceCapitalLetters()} Edit View because of an error on the server.") : Problem();
        }
    }

    /// <inheritdoc/>
    /// <remarks>Overridden to redirect to the work order template's index instead of the schedule's index.</remarks>
    public override async Task<IActionResult> UpdateAsync(WorkOrderTemplateSchedule dataObject)
    {
        IActionResult actionResult = await base.UpdateAsync(dataObject);

        if (actionResult is RedirectToActionResult redirectToActionResult && redirectToActionResult.ActionName == nameof(Index))
        {
            redirectToActionResult.ControllerName = nameof(WorkOrderTemplate);
        }

        return actionResult;
    }
}
