using JMayer.Example.ASPVanillaMVC.Models;
using Microsoft.AspNetCore.Mvc.Testing;
using System.Net;

namespace TestProject.Test.WebRequest;

/// <summary>
/// The class manages tests the work order template schedule controller using both a http client and the server.
/// </summary>
/// <remarks>
/// The example web server creates default data objects and the unit tests
/// uses this already existing data.
/// </remarks>
public class WorkOrderTemplateScheduleUnitTest : IClassFixture<WebApplicationFactory<Program>>
{
    /// <summary>
    /// The factory for the web application.
    /// </summary>
    private readonly WebApplicationFactory<Program> _factory;

    /// <summary>
    /// The dependency injection constructor.
    /// </summary>
    /// <param name="factory">The factory for the web application.</param>
    public WorkOrderTemplateScheduleUnitTest(WebApplicationFactory<Program> factory) => _factory = factory;

    /// <summary>
    /// The method verifies the work order template schedule will automatically be created when the template is created.
    /// </summary>
    /// <returns>A Task for the async.</returns>
    /// <remarks>Because there is a one to one relationship, the template id should always equal to the schedule id.</remarks>
    [Fact]
    public async Task VerifyCreateWorkOrderTemplateScheduleCascade()
    {
        HttpClient httpClient = _factory.CreateClient();
        long? id = await DataHelper.CreateWorkOrderTemplateAsync(httpClient, "Create Work Order Template Cascade Test");

        if (id is null)
        {
            Assert.Fail(Constants.CreateTemplateFailureMessage);
        }

        HttpResponseMessage httpResponseMessage = await httpClient.GetAsync($"WorkOrderTemplateSchedule/EditView/{id}");

        Assert.True(httpResponseMessage.IsSuccessStatusCode, Constants.NonSuccessfulResponseFailureMessage); //The operation must have been successful.
        Assert.NotEqual(HttpStatusCode.NoContent, httpResponseMessage.StatusCode); //Content must have been returned.

        string html = await httpResponseMessage.Content.ReadAsStringAsync();

        //HTML must have been returned.
        Assert.NotEmpty(html);
        Assert.StartsWith(Constants.DocTypeHtmlTag, html);
        Assert.DoesNotContain(Constants.ErrorHtmlSearchTag, html);
        Assert.DoesNotContain(Constants.NotFoundHtmlSearchTag, html);
    }

    /// <summary>
    /// The method verifies the work order template schedule will automatically be deleted when the template is deleted.
    /// </summary>
    /// <returns>A Task for the async.</returns>
    /// <remarks>Because there is a one to one relationship, the template id should always equal to the schedule id.</remarks>
    [Fact]
    public async Task VerifyDeleteWorkOrderTemplateScheduleCascade()
    {
        HttpClient httpClient = _factory.CreateClient();
        long? id = await DataHelper.CreateWorkOrderTemplateAsync(httpClient, "Delete Work Order Template Cascade Test");

        if (id is null)
        {
            Assert.Fail(Constants.CreateTemplateFailureMessage);
        }

        _ = await httpClient.PostAsync($"WorkOrderTemplate/Delete/{id}", new StringContent(string.Empty));
        HttpResponseMessage httpResponseMessage = await httpClient.GetAsync($"WorkOrderTemplateSchedule/EditView/{id}");

        Assert.True(httpResponseMessage.IsSuccessStatusCode, Constants.NonSuccessfulResponseFailureMessage); //The operation must have been successful.
        Assert.NotEqual(HttpStatusCode.NoContent, httpResponseMessage.StatusCode); //Content must have been returned.

        string html = await httpResponseMessage.Content.ReadAsStringAsync();

        //HTML must have been returned.
        Assert.NotEmpty(html);
        Assert.StartsWith(Constants.DocTypeHtmlTag, html);
        Assert.Contains(Constants.NotFoundHtmlSearchTag, html);
    }

    /// <summary>
    /// The method verifies the work order template schedule controller can return the edit view when requested.
    /// </summary>
    /// <returns>A Task for the async.</returns>
    [Fact]
    public async Task VerifyEditView()
    {
        HttpClient httpClient = _factory.CreateClient();
        HttpResponseMessage httpResponseMessage = await httpClient.GetAsync("WorkOrderTemplateSchedule/EditView/1");

        Assert.True(httpResponseMessage.IsSuccessStatusCode, Constants.NonSuccessfulResponseFailureMessage); //The operation must have been successful.
        Assert.NotEqual(HttpStatusCode.NoContent, httpResponseMessage.StatusCode); //Content must have been returned.

        string html = await httpResponseMessage.Content.ReadAsStringAsync();

        //HTML must have been returned.
        Assert.NotEmpty(html);
        Assert.StartsWith(Constants.DocTypeHtmlTag, html);
        Assert.DoesNotContain(Constants.ErrorHtmlSearchTag, html);
        Assert.DoesNotContain(Constants.NotFoundHtmlSearchTag, html);
    }

    /// <summary>
    /// The method verifies the work order template schedule controller returns the not found view when the requested id does not exist.
    /// </summary>
    /// <returns>A Task for the async.</returns>
    [Fact]
    public async Task VerifyEditViewNotFound()
    {
        HttpClient httpClient = _factory.CreateClient();
        HttpResponseMessage httpResponseMessage = await httpClient.GetAsync("WorkOrderTemplateSchedule/EditView/999999");

        Assert.True(httpResponseMessage.IsSuccessStatusCode, Constants.NonSuccessfulResponseFailureMessage); //The operation must have been successful.
        Assert.NotEqual(HttpStatusCode.NoContent, httpResponseMessage.StatusCode); //Content must have been returned.

        string html = await httpResponseMessage.Content.ReadAsStringAsync();

        //HTML must have been returned.
        Assert.NotEmpty(html);
        Assert.StartsWith(Constants.DocTypeHtmlTag, html);
        Assert.Contains(Constants.NotFoundHtmlSearchTag, html);
    }

    /// <summary>
    /// The method verifies the work order template schedule controller can update a work order template schedule when requested by the user.
    /// </summary>
    /// <param name="scheduleType">The frequency for the schedule.</param>
    /// <param name="enabled">Whether or not the scheduler considers this work order template and schedule.</param>
    /// <returns>A Task for the async.</returns>
    [Theory]
    [InlineData(WorkOrderTemplateScheduleType.Daily, true)]
    [InlineData(WorkOrderTemplateScheduleType.Weekly, false)]
    [InlineData(WorkOrderTemplateScheduleType.Monthly, true)]
    [InlineData(WorkOrderTemplateScheduleType.Quarterly, false)]
    [InlineData(WorkOrderTemplateScheduleType.Semiyearly, true)]
    [InlineData(WorkOrderTemplateScheduleType.Yearly, false)]
    public async Task VerifyUpdateWorkOrderTemplateSchedule(WorkOrderTemplateScheduleType scheduleType, bool enabled)
    {
        HttpClient client = _factory.CreateClient();
        long? id = await DataHelper.CreateWorkOrderTemplateAsync(client, $"Update Work Order Template Schedule {scheduleType}");

        if (id is null)
        {
            Assert.Fail(Constants.CreateTemplateFailureMessage);
        }

        HttpClient httpClient = _factory.CreateClient();
        FormUrlEncodedContent content = DataHelper.CreateWorkOrderTemplateScheduleFormUrlEncodedContent(id.Value, scheduleType, enabled);
        HttpResponseMessage httpResponseMessage = await httpClient.PostAsync("WorkOrderTemplateSchedule/Update", content);

        Assert.True(httpResponseMessage.IsSuccessStatusCode, Constants.NonSuccessfulResponseFailureMessage); //The operation must have been successful.
        Assert.NotEqual(HttpStatusCode.NoContent, httpResponseMessage.StatusCode); //Content must have been returned.

        string html = await httpResponseMessage.Content.ReadAsStringAsync();

        //HTML must have been returned.
        Assert.NotEmpty(html);
        Assert.StartsWith(Constants.DocTypeHtmlTag, html);
        Assert.DoesNotContain(Constants.ConflictHtmlSearchTag, html);
        Assert.DoesNotContain(Constants.ErrorHtmlSearchTag, html);
        Assert.DoesNotContain(Constants.NotFoundHtmlSearchTag, html);
        Assert.Contains($"Update Work Order Template Schedule {scheduleType}", html);
    }

    /// <summary>
    /// The method verifies the work order template schedule controller will return a not found when it receives an update request 
    /// by the user and the work order template schedule doesn't exist.
    /// </summary>
    /// <returns>A Task for the async.</returns>
    [Fact]
    public async Task VerifyUpdateWorkOrderTemplateScheduleNotFound()
    {
        HttpClient httpClient = _factory.CreateClient();
        FormUrlEncodedContent content = DataHelper.CreateWorkOrderTemplateScheduleFormUrlEncodedContent(999999, WorkOrderTemplateScheduleType.Monthly, true);
        HttpResponseMessage httpResponseMessage = await httpClient.PostAsync("WorkOrderTemplateSchedule/Update", content);

        Assert.True(httpResponseMessage.IsSuccessStatusCode, Constants.NonSuccessfulResponseFailureMessage); //The operation must have been successful.
        Assert.NotEqual(HttpStatusCode.NoContent, httpResponseMessage.StatusCode); //Content must have been returned.

        string html = await httpResponseMessage.Content.ReadAsStringAsync();

        //HTML must have been returned.
        Assert.NotEmpty(html);
        Assert.StartsWith(Constants.DocTypeHtmlTag, html);
        Assert.Contains(Constants.NotFoundHtmlSearchTag, html);
    }

    /// <summary>
    /// The method verifies the work order template schedule controller will return a conflict when two users try to update the same
    /// work order template schedule.
    /// </summary>
    /// <returns>A Task for the async.</returns>
    [Fact]
    public async Task VerifyUpdateWorkOrderTemplateScheduleOldDataConflict()
    {
        HttpClient httpClient = _factory.CreateClient();
        long? id = await DataHelper.CreateWorkOrderTemplateAsync(httpClient, "Update Work Order Template Schedule Old Data Conflict Test");

        if (id is null)
        {
            Assert.Fail(Constants.CreateTemplateFailureMessage);
        }

        FormUrlEncodedContent content = DataHelper.CreateWorkOrderTemplateScheduleFormUrlEncodedContent(id.Value, WorkOrderTemplateScheduleType.Monthly, true);
        HttpResponseMessage httpResponseMessage = await httpClient.PostAsync("WorkOrderTemplateSchedule/Update", content);

        Assert.True(httpResponseMessage.IsSuccessStatusCode, Constants.NonSuccessfulResponseFailureMessage); //The operation must have been successful.
        Assert.NotEqual(HttpStatusCode.NoContent, httpResponseMessage.StatusCode); //Content must have been returned.

        string html = await httpResponseMessage.Content.ReadAsStringAsync();

        //HTML must have been returned.
        Assert.NotEmpty(html);
        Assert.StartsWith(Constants.DocTypeHtmlTag, html);
        Assert.DoesNotContain(Constants.ConflictHtmlSearchTag, html);
        Assert.DoesNotContain(Constants.ErrorHtmlSearchTag, html);
        Assert.DoesNotContain(Constants.NotFoundHtmlSearchTag, html);
        Assert.Contains("Update Work Order Template Schedule Old Data Conflict Test", html);

        httpResponseMessage = await httpClient.PostAsync("WorkOrderTemplateSchedule/Update", content);

        Assert.True(httpResponseMessage.IsSuccessStatusCode, Constants.NonSuccessfulResponseFailureMessage); //The operation must have been successful.
        Assert.NotEqual(HttpStatusCode.NoContent, httpResponseMessage.StatusCode); //Content must have been returned.

        html = await httpResponseMessage.Content.ReadAsStringAsync();

        //HTML must have been returned.
        Assert.NotEmpty(html);
        Assert.StartsWith(Constants.DocTypeHtmlTag, html);
        Assert.Contains(Constants.ConflictHtmlSearchTag, html);
    }
}
