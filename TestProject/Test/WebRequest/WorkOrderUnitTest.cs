using Microsoft.AspNetCore.Mvc.Testing;
using System.Net;

namespace TestProject.Test.WebRequest;

/// <summary>
/// The class manages tests the work order controller using both a http client and the server.
/// </summary>
/// <remarks>
/// The example web server creates default data objects and the unit tests
/// uses this already existing data.
/// </remarks>
public class WorkOrderUnitTest : IClassFixture<WebApplicationFactory<Program>>
{
    /// <summary>
    /// The factory for the web application.
    /// </summary>
    private readonly WebApplicationFactory<Program> _factory;

    /// <summary>
    /// The dependency injection constructor.
    /// </summary>
    /// <param name="factory">The factory for the web application.</param>
    public WorkOrderUnitTest(WebApplicationFactory<Program> factory) => _factory = factory;

    /// <summary>
    /// The method verifies the work order template controller can return the index view.
    /// </summary>
    /// <returns>A Task for the async.</returns>
    [Fact]
    public async Task VerifyIndexView()
    {
        HttpClient client = _factory.CreateClient();
        HttpResponseMessage httpResponseMessage = await client.GetAsync("WorkOrder/Index");

        Assert.True(httpResponseMessage.IsSuccessStatusCode, Constants.NonSuccessfulResponseFailureMessage); //The operation must have been successful.
        Assert.NotEqual(HttpStatusCode.NoContent, httpResponseMessage.StatusCode); //Content must have been returned.

        string html = await httpResponseMessage.Content.ReadAsStringAsync();

        //HTML must have been returned.
        Assert.NotEmpty(html);
        Assert.StartsWith(Constants.DocTypeHtmlTag, html);
        Assert.DoesNotContain(Constants.ErrorHtmlSearchTag, html);
    }
}
