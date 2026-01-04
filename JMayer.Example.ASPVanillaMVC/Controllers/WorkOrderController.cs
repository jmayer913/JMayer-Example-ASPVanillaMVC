using JMayer.Example.ASPVanillaMVC.DataLayers;
using JMayer.Example.ASPVanillaMVC.Models;
using JMayer.Web.Mvc.Controller.Mvc;

namespace JMayer.Example.ASPVanillaMVC.Controllers;

/// <summary>
/// The class manages HTTP requests for views and actions associated with work orders.
/// </summary>
public class WorkOrderController : StandardModelViewController<WorkOrder, IWorkOrderDataLayer>
{
    /// <inheritdoc/>
    public WorkOrderController(IWorkOrderDataLayer dataLayer, ILogger<WorkOrderController> logger) : base(dataLayer, logger) { }
}
