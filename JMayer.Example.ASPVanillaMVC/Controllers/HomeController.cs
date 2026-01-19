using System.Net;
using Microsoft.AspNetCore.Mvc;

namespace JMayer.Example.ASPVanillaMVC.Controllers;

/// <summary>
/// The class manages HTTP requests for views and actions associated with the home page.
/// </summary>
public class HomeController : Controller
{
    /// <summary>
    /// Used to log activity in the controller.
    /// </summary>
    private readonly ILogger<HomeController> _logger;

    /// <summary>
    /// The dependency injection constructor.
    /// </summary>
    /// <param name="logger">Used to log activity in the controller.</param>
    public HomeController(ILogger<HomeController> logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// The method returns the home's index view.
    /// </summary>
    /// <returns>The home's index view.</returns>
    public IActionResult Index()
    {
        return View();
    }

    /// <summary>
    /// The method returns a view based on the status code.
    /// </summary>
    /// <param name="statusCode">The status code.</param>
    /// <returns>A view.</returns>
    [Route("Home/Error/{statusCode}")]
    public IActionResult Error(int? statusCode)
    {
        return statusCode switch
        {
            (int)HttpStatusCode.Conflict => View("UpdateConflict"),
            (int)HttpStatusCode.NotFound => View("NotFound"),
            _ => View("Error")
        };
    }
}
