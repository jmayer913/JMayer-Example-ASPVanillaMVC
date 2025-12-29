using JMayer.Example.ASPVanillaMVC.DataLayers;
using JMayer.Example.ASPVanillaMVC.Models;
using JMayer.Web.Mvc.Controller.Mvc;

namespace JMayer.Example.ASPVanillaMVC.Controllers;

/// <summary>
/// The class manages HTTP requests for views and actions associated with work order templates.
/// </summary>
public class WorkOrderTemplateController : StandardModelViewController<WorkOrderTemplate, IWorkOrderTemplateDataLayer>
{
    /// <inheritdoc/>
    public WorkOrderTemplateController(IWorkOrderTemplateDataLayer dataLayer, ILogger<WorkOrderTemplateController> logger) : base(dataLayer, logger) { }
}
