using JMayer.Data.HTTP.Details;
using JMayer.Example.ASPVanillaMVC.Models;
using Microsoft.AspNetCore.Mvc.Testing;
using System.Net;
using System.Net.Http.Json;
using System.Xml.Linq;

namespace TestProject.Test.WebRequest;

/// <summary>
/// The class manages tests the work order template controller using both a http client and the server.
/// </summary>
/// <remarks>
/// The example web server creates default data objects and the unit tests
/// uses this already existing data.
/// </remarks>
public class WorkOrderTemplateUnitTest : IClassFixture<WebApplicationFactory<Program>>
{
    /// <summary>
    /// The factory for the web application.
    /// </summary>
    private readonly WebApplicationFactory<Program> _factory;

    /// <summary>
    /// The constant for the conflict header when searching in html.
    /// </summary>
    private const string ConflictHtmlSearchTag = "<h3>Sorry, the submitted data was detected to be out of date; please go back to the list page and try editing again.</h3>";

    /// <summary>
    /// The constant for the doctype tag in html.
    /// </summary>
    private const string DocTypeHtmlTag = "<!DOCTYPE html>";

    /// <summary>
    /// The constant for the error header when searching in html.
    /// </summary>
    private const string ErrorHtmlSearchTag = "<h3>Sorry, an unexpected error occurred.</h3>";

    /// <summary>
    /// The constant for the not found header when searching in html.
    /// </summary>
    private const string NotFoundHtmlSearchTag = "<h3>Sorry, the page or resource was not found.</h3>";

    /// <summary>
    /// The dependency injection constructor.
    /// </summary>
    /// <param name="factory">The factory for the web application.</param>
    public WorkOrderTemplateUnitTest(WebApplicationFactory<Program> factory) => _factory = factory;

    /// <summary>
    /// The method creates a work order template on the remote web server.
    /// </summary>
    /// <param name="httpClient">Used to communicate with the web server.</param>
    /// <param name="name">The name of the work order template.</param>
    /// <returns>The created work order template.</returns>
    private static async Task<long?> CreateWorkOrderTemplateAsync(HttpClient httpClient, string name)
    {
        Dictionary<string, string> values = new()
        {
            { "daysDueFromCreation", "0" },
            { "description", string.Empty },
            { "name", name },
            { "otherTypeOfService", string.Empty },
            { "priority", ((int)WorkOrderPriority.Normal).ToString() },
            { "serviceType", ((int)WorkOrderServiceType.Inspection).ToString() },
        };
        FormUrlEncodedContent content = new(values);

        HttpResponseMessage httpResponseMessage = await httpClient.PostAsync("WorkOrderTemplate/Create", content);

        if (httpResponseMessage.IsSuccessStatusCode is false || httpResponseMessage.StatusCode is HttpStatusCode.NoContent)
        {
            return null;
        }

        string html = await httpResponseMessage.Content.ReadAsStringAsync();

        if (string.IsNullOrEmpty(html))
        {
            return null;
        }

        int startIndex = html.LastIndexOf("/WorkOrderTemplate/EditView/");

        if (startIndex == -1)
        {
            return null;
        }

        int endIndex = html.IndexOf('\"', startIndex);

        if (endIndex == -1)
        {
            return null;
        }

        string id = html[startIndex..endIndex].Split('/').Last();

        return Convert.ToInt64(id);
    }

    /// <summary>
    /// The method verifies the work order template controller can return the add view when requested by the user.
    /// </summary>
    /// <returns>A Task for the async.</returns>
    [Fact]
    public async Task VerifyAddView()
    {
        HttpClient client = _factory.CreateClient();
        HttpResponseMessage httpResponseMessage = await client.GetAsync("WorkOrderTemplate/AddView");

        Assert.True(httpResponseMessage.IsSuccessStatusCode, "The operation should have been successful."); //The operation must have been successful.
        Assert.NotEqual(HttpStatusCode.NoContent, httpResponseMessage.StatusCode); //Content must have been returned.

        string html = await httpResponseMessage.Content.ReadAsStringAsync();

        //HTML must have been returned.
        Assert.NotEmpty(html);
        Assert.StartsWith(DocTypeHtmlTag, html);
        Assert.DoesNotContain(ErrorHtmlSearchTag, html);
    }

    /// <summary>
    /// The method verifies the work order controller can create a work order when requested by the user.
    /// </summary>
    /// <param name="name">The friendly name for the work order.</param>
    /// <param name="description">A description for the work order.</param>
    /// <param name="serviceType">The type of service for the work order.</param>
    /// <param name="otherTypeOfService">A custom description for the type when the standard (Inspection, Routine & Reactive) does not apply.</param>
    /// <param name="priority">The priority for the work order.</param>
    /// <param name="daysDueFromCreation">The number of days the work order will be due based on when its created.</param>
    /// <returns>A Task for the async.</returns>
    [Theory]
    [InlineData("Create Work Order Template Test Inspection", "Insepction Description", WorkOrderServiceType.Inspection, null, WorkOrderPriority.Low, 0)]
    [InlineData("Create Work Order Template Test Routine", "Routine Description", WorkOrderServiceType.Routine, null, WorkOrderPriority.Normal, 7)]
    [InlineData("Create Work Order Template Test Reactive", "Reactive Description", WorkOrderServiceType.Reactive, null, WorkOrderPriority.High, 30)]
    [InlineData("Create Work Order Template Test Other", "Other Description", WorkOrderServiceType.Other, "Other", WorkOrderPriority.High, 60)]
    public async Task VerifyCreateWorkOrderTemplate(string name, string description, WorkOrderServiceType serviceType, string? otherTypeOfService, WorkOrderPriority priority, int daysDueFromCreation)
    {
        Dictionary<string, string> formValues = new()
        {
            { "daysDueFromCreation", daysDueFromCreation.ToString() },
            { "description", description },
            { "name", name },
            { "otherTypeOfService", otherTypeOfService ?? string.Empty },
            { "priority", ((int)priority).ToString() },
            { "serviceType", ((int)serviceType).ToString() },
        };
        FormUrlEncodedContent content = new(formValues);

        HttpClient httpClient = _factory.CreateClient();
        HttpResponseMessage httpResponseMessage = await httpClient.PostAsync("WorkOrderTemplate/Create", content);

        Assert.True(httpResponseMessage.IsSuccessStatusCode, "The operation should have been successful."); //The operation must have been successful.
        Assert.NotEqual(HttpStatusCode.NoContent, httpResponseMessage.StatusCode); //Content must have been returned.

        string html = await httpResponseMessage.Content.ReadAsStringAsync();

        //HTML must have been returned.
        Assert.NotEmpty(html);
        Assert.StartsWith(DocTypeHtmlTag, html);
        Assert.DoesNotContain(ErrorHtmlSearchTag, html);
        Assert.DoesNotContain(NotFoundHtmlSearchTag, html);
        Assert.Contains(name, html);
    }

    /// <summary>
    /// The method verifies the work order template controller will return html with a validation error when it receives a create requested 
    /// but another work order has the same name.
    /// </summary>
    /// <returns>A Task for the async.</returns>
    [Fact]
    public async Task VerifyCreateWorkOrderTemplateDuplicateFailure()
    {
        HttpClient httpClient = _factory.CreateClient();
        long? id = await CreateWorkOrderTemplateAsync(httpClient, "Create Work Order Template Duplicate Test");

        if (id is null)
        {
            Assert.Fail("Failed to create a work order template for the test.");
        }

        Dictionary<string, string> formValues = new()
        {
            { "daysDueFromCreation", "0" },
            { "description", string.Empty },
            { "name", "Create Work Order Template Duplicate Test" },
            { "otherTypeOfService", string.Empty },
            { "priority", ((int)WorkOrderPriority.Normal).ToString() },
            { "serviceType", ((int)WorkOrderServiceType.Inspection).ToString() },
        };
        FormUrlEncodedContent content = new(formValues);

        HttpResponseMessage httpResponseMessage = await httpClient.PostAsync("WorkOrderTemplate/Create", content);

        Assert.True(httpResponseMessage.IsSuccessStatusCode, "The operation should have been successful."); //The operation must have been successful.
        Assert.NotEqual(HttpStatusCode.NoContent, httpResponseMessage.StatusCode); //Content must have been returned.

        string html = await httpResponseMessage.Content.ReadAsStringAsync();

        //HTML must have been returned.
        Assert.NotEmpty(html);
        Assert.StartsWith(DocTypeHtmlTag, html);
        Assert.Contains("<span class=\"text-danger field-validation-error\" data-valmsg-for=\"Name\" data-valmsg-replace=\"true\">The Create Work Order Template Duplicate Test name already exists in the data store.</span>", html);
    }

    /// <summary>
    /// The method verifies the work order template controller will return html with a validation error when it receives a create requested 
    /// by the user and the work order template has no name.
    /// </summary>
    /// <returns>A Task for the async.</returns>
    [Fact]
    public async Task VerifyCreateWorkOrderTemplateNameRequiredValidationFailure()
    {
        Dictionary<string, string> formValues = new()
        {
            { "daysDueFromCreation", "0" },
            { "description", string.Empty },
            { "name", string.Empty },
            { "otherTypeOfService", string.Empty },
            { "priority", ((int)WorkOrderPriority.Normal).ToString() },
            { "serviceType", ((int)WorkOrderServiceType.Inspection).ToString() },
        };
        FormUrlEncodedContent content = new(formValues);

        HttpClient httpClient = _factory.CreateClient();
        HttpResponseMessage httpResponseMessage = await httpClient.PostAsync("WorkOrderTemplate/Create", content);

        Assert.True(httpResponseMessage.IsSuccessStatusCode, "The operation should have been successful."); //The operation must have been successful.
        Assert.NotEqual(HttpStatusCode.NoContent, httpResponseMessage.StatusCode); //Content must have been returned.

        string html = await httpResponseMessage.Content.ReadAsStringAsync();

        //HTML must have been returned.
        Assert.NotEmpty(html);
        Assert.StartsWith(DocTypeHtmlTag, html);
        Assert.Contains("<span class=\"text-danger field-validation-error\" data-valmsg-for=\"Name\" data-valmsg-replace=\"true\">The Name field is required.</span>", html);
    }

    /// <summary>
    /// The method verifies the work order controller can delete a work order when requested by the user.
    /// </summary>
    /// <returns></returns>
    [Fact]
    public async Task VerifyDeleteWorkOrderTemplate()
    {
        HttpClient httpClient = _factory.CreateClient();
        long? id = await CreateWorkOrderTemplateAsync(httpClient, "Delete Work Order Template Test");

        if (id is null)
        {
            Assert.Fail("Failed to create a work order template for the test.");
        }

        HttpResponseMessage httpResponseMessage = await httpClient.PostAsync($"WorkOrderTemplate/Delete/{id}", new StringContent(string.Empty));

        Assert.True(httpResponseMessage.IsSuccessStatusCode, "The operation should have been successful."); //The operation must have been successful.
        Assert.NotEqual(HttpStatusCode.NoContent, httpResponseMessage.StatusCode); //Content must have been returned.

        string html = await httpResponseMessage.Content.ReadAsStringAsync();

        //HTML must have been returned.
        Assert.NotEmpty(html);
        Assert.StartsWith(DocTypeHtmlTag, html);
        Assert.DoesNotContain(ConflictHtmlSearchTag, html);
        Assert.DoesNotContain(ErrorHtmlSearchTag, html);
        Assert.DoesNotContain(NotFoundHtmlSearchTag, html);
    }

    /// <summary>
    /// The method verifies the work order controller can delete a work order when requested by the user.
    /// </summary>
    /// <returns></returns>
    [Fact]
    public async Task VerifyDeleteWorkOrderTemplateNotFound()
    {
        HttpClient httpClient = _factory.CreateClient();
        HttpResponseMessage httpResponseMessage = await httpClient.PostAsync("WorkOrderTemplate/Delete/999999", new StringContent(string.Empty));

        Assert.True(httpResponseMessage.IsSuccessStatusCode, "The operation should have been successful."); //The operation must have been successful.
        Assert.NotEqual(HttpStatusCode.NoContent, httpResponseMessage.StatusCode); //Content must have been returned.

        string html = await httpResponseMessage.Content.ReadAsStringAsync();

        //HTML must have been returned.
        Assert.NotEmpty(html);
        Assert.StartsWith(DocTypeHtmlTag, html);
        Assert.Contains(NotFoundHtmlSearchTag, html);
    }

    /// <summary>
    /// The method verifies the work order template controller can return the delete view when requested by the user.
    /// </summary>
    /// <returns>A Task for the async.</returns>
    [Fact]
    public async Task VerifyDeleteView()
    {
        HttpClient client = _factory.CreateClient();
        HttpResponseMessage httpResponseMessage = await client.GetAsync("WorkOrderTemplate/DeleteView/1");

        Assert.True(httpResponseMessage.IsSuccessStatusCode, "The operation should have been successful."); //The operation must have been successful.
        Assert.NotEqual(HttpStatusCode.NoContent, httpResponseMessage.StatusCode); //Content must have been returned.

        string html = await httpResponseMessage.Content.ReadAsStringAsync();

        //HTML must have been returned.
        Assert.NotEmpty(html); 
        Assert.StartsWith(DocTypeHtmlTag, html);
        Assert.DoesNotContain(ErrorHtmlSearchTag, html);
        Assert.DoesNotContain(NotFoundHtmlSearchTag, html);
    }

    /// <summary>
    /// The method verifies the work order template controller returns the not found view when the requested id does not exist.
    /// </summary>
    /// <returns>A Task for the async.</returns>
    [Fact]
    public async Task VerifyDeleteViewNotFound()
    {
        HttpClient client = _factory.CreateClient();
        HttpResponseMessage httpResponseMessage = await client.GetAsync("WorkOrderTemplate/DeleteView/999999");

        Assert.True(httpResponseMessage.IsSuccessStatusCode, "The operation should have been successful."); //The operation must have been successful.
        Assert.NotEqual(HttpStatusCode.NoContent, httpResponseMessage.StatusCode); //Content must have been returned.

        string html = await httpResponseMessage.Content.ReadAsStringAsync();

        //HTML must have been returned.
        Assert.NotEmpty(html);
        Assert.StartsWith(DocTypeHtmlTag, html);
        Assert.Contains(NotFoundHtmlSearchTag, html);
    }

    /// <summary>
    /// The method verifies the work order template controller can return the edit view when requested.
    /// </summary>
    /// <returns>A Task for the async.</returns>
    [Fact]
    public async Task VerifyEditView()
    {
        HttpClient client = _factory.CreateClient();
        HttpResponseMessage httpResponseMessage = await client.GetAsync("WorkOrderTemplate/EditView/1");

        Assert.True(httpResponseMessage.IsSuccessStatusCode, "The operation should have been successful."); //The operation must have been successful.
        Assert.NotEqual(HttpStatusCode.NoContent, httpResponseMessage.StatusCode); //Content must have been returned.

        string html = await httpResponseMessage.Content.ReadAsStringAsync();

        //HTML must have been returned.
        Assert.NotEmpty(html);
        Assert.StartsWith(DocTypeHtmlTag, html);
        Assert.DoesNotContain(ErrorHtmlSearchTag, html);
        Assert.DoesNotContain(NotFoundHtmlSearchTag, html);
    }

    /// <summary>
    /// The method verifies the work order template controller returns the not found view when the requested id does not exist.
    /// </summary>
    /// <returns>A Task for the async.</returns>
    [Fact]
    public async Task VerifyEditViewNotFound()
    {
        HttpClient client = _factory.CreateClient();
        HttpResponseMessage httpResponseMessage = await client.GetAsync("WorkOrderTemplate/EditView/999999");

        Assert.True(httpResponseMessage.IsSuccessStatusCode, "The operation should have been successful."); //The operation must have been successful.
        Assert.NotEqual(HttpStatusCode.NoContent, httpResponseMessage.StatusCode); //Content must have been returned.

        string html = await httpResponseMessage.Content.ReadAsStringAsync();

        //HTML must have been returned.
        Assert.NotEmpty(html);
        Assert.StartsWith(DocTypeHtmlTag, html);
        Assert.Contains(NotFoundHtmlSearchTag, html);
    }

    /// <summary>
    /// The method verifies the work order template controller can return the index view.
    /// </summary>
    /// <returns>A Task for the async.</returns>
    [Fact]
    public async Task VerifyIndexView()
    {
        HttpClient client = _factory.CreateClient();
        HttpResponseMessage httpResponseMessage = await client.GetAsync("WorkOrderTemplate/Index");

        Assert.True(httpResponseMessage.IsSuccessStatusCode, "The operation should have been successful."); //The operation must have been successful.
        Assert.NotEqual(HttpStatusCode.NoContent, httpResponseMessage.StatusCode); //Content must have been returned.

        string html = await httpResponseMessage.Content.ReadAsStringAsync();

        //HTML must have been returned.
        Assert.NotEmpty(html);
        Assert.StartsWith(DocTypeHtmlTag, html);
        Assert.DoesNotContain(ErrorHtmlSearchTag, html);
    }

    /// <summary>
    /// The method verifies the work order controller can update a work order when requested by the user.
    /// </summary>
    /// <param name="name">The friendly name for the work order.</param>
    /// <param name="description">A description for the work order.</param>
    /// <param name="serviceType">The type of service for the work order.</param>
    /// <param name="otherTypeOfService">A custom description for the type when the standard (Inspection, Routine & Reactive) does not apply.</param>
    /// <param name="priority">The priority for the work order.</param>
    /// <param name="daysDueFromCreation">The number of days the work order will be due based on when its created.</param>
    /// <returns>A Task for the async.</returns>
    [Theory]
    [InlineData("Update Work Order Template Test Inspection", "Update Work Order Test Inspection Renamed", "Insepction Description", WorkOrderServiceType.Inspection, null, WorkOrderPriority.Low, 0)]
    [InlineData("Update Work Order Template Test Routine", "Update Work Order Test Routine Renamed", "Routine Description", WorkOrderServiceType.Routine, null, WorkOrderPriority.Normal, 7)]
    [InlineData("Update Work Order Template Test Reactive", "Update Work Order Test Reactive Renamed", "Reactive Description", WorkOrderServiceType.Reactive, null, WorkOrderPriority.High, 30)]
    [InlineData("Update Work Order Template Test Other", "Update Work Order Test Other Renamed", "Other Description", WorkOrderServiceType.Other, "Other", WorkOrderPriority.High, 60)]
    public async Task VerifyUpdateWorkOrderTemplate(string originalName, string newName, string description, WorkOrderServiceType serviceType, string? otherTypeOfService, WorkOrderPriority priority, int daysDueFromCreation)
    {
        HttpClient client = _factory.CreateClient();
        long? id = await CreateWorkOrderTemplateAsync(client, originalName);

        if (id is null)
        {
            Assert.Fail("Failed to create a work order template for the test.");
        }

        Dictionary<string, string> formValues = new()
        {
            { "daysDueFromCreation", daysDueFromCreation.ToString() },
            { "description", description },
            { "name", newName },
            { "integer64ID", id.Value.ToString() },
            { "otherTypeOfService", otherTypeOfService ?? string.Empty },
            { "priority", ((int)priority).ToString() },
            { "serviceType", ((int)serviceType).ToString() },
        };
        FormUrlEncodedContent content = new(formValues);

        HttpClient httpClient = _factory.CreateClient();
        HttpResponseMessage httpResponseMessage = await httpClient.PostAsync("WorkOrderTemplate/Update", content);

        Assert.True(httpResponseMessage.IsSuccessStatusCode, "The operation should have been successful."); //The operation must have been successful.
        Assert.NotEqual(HttpStatusCode.NoContent, httpResponseMessage.StatusCode); //Content must have been returned.

        string html = await httpResponseMessage.Content.ReadAsStringAsync();

        //HTML must have been returned.
        Assert.NotEmpty(html);
        Assert.StartsWith(DocTypeHtmlTag, html);
        Assert.DoesNotContain(ConflictHtmlSearchTag, html);
        Assert.DoesNotContain(ErrorHtmlSearchTag, html);
        Assert.DoesNotContain(NotFoundHtmlSearchTag, html);
        Assert.Contains(newName, html);
    }

    /// <summary>
    /// The method verifies the work order template controller will return html with a validation error when it receives a create requested 
    /// but another work order has the same name.
    /// </summary>
    /// <returns>A Task for the async.</returns>
    [Fact]
    public async Task VerifyUpdateWorkOrderTemplateDuplicateFailure()
    {
        HttpClient httpClient = _factory.CreateClient();
        long? id = await CreateWorkOrderTemplateAsync(httpClient, "Update Work Order Template Duplicate Test 1");

        if (id is null)
        {
            Assert.Fail("Failed to create a work order template for the test.");
        }

        id = await CreateWorkOrderTemplateAsync(httpClient, "Update Work Order Template Duplicate Test 2");

        if (id is null)
        {
            Assert.Fail("Failed to create a work order template for the test.");
        }

        Dictionary<string, string> formValues = new()
        {
            { "daysDueFromCreation", "0" },
            { "description", string.Empty },
            { "name", "Update Work Order Template Duplicate Test 1" },
            { "integer64ID", id.Value.ToString() },
            { "otherTypeOfService", string.Empty },
            { "priority", ((int)WorkOrderPriority.Normal).ToString() },
            { "serviceType", ((int)WorkOrderServiceType.Inspection).ToString() },
        };
        FormUrlEncodedContent content = new(formValues);

        HttpResponseMessage httpResponseMessage = await httpClient.PostAsync("WorkOrderTemplate/Create", content);

        Assert.True(httpResponseMessage.IsSuccessStatusCode, "The operation should have been successful."); //The operation must have been successful.
        Assert.NotEqual(HttpStatusCode.NoContent, httpResponseMessage.StatusCode); //Content must have been returned.

        string html = await httpResponseMessage.Content.ReadAsStringAsync();

        //HTML must have been returned.
        Assert.NotEmpty(html);
        Assert.StartsWith(DocTypeHtmlTag, html);
        Assert.Contains("<span class=\"text-danger field-validation-error\" data-valmsg-for=\"Name\" data-valmsg-replace=\"true\">The Update Work Order Template Duplicate Test 1 name already exists in the data store.</span>", html);
    }

    /// <summary>
    /// The method verifies the work order template controller will return html with a validation error when it receives a update requested 
    /// by the user and the work order template has no name.
    /// </summary>
    /// <returns>A Task for the async.</returns>
    [Fact]
    public async Task VerifyUpdateWorkOrderTemplateNameRequiredValidationFailure()
    {
        Dictionary<string, string> formValues = new()
        {
            { "daysDueFromCreation", "0" },
            { "description", string.Empty },
            { "name", string.Empty },
            { "integer64ID", "1" },
            { "otherTypeOfService", string.Empty },
            { "priority", ((int)WorkOrderPriority.Normal).ToString() },
            { "serviceType", ((int)WorkOrderServiceType.Inspection).ToString() },
        };
        FormUrlEncodedContent content = new(formValues);

        HttpClient httpClient = _factory.CreateClient();
        HttpResponseMessage httpResponseMessage = await httpClient.PostAsync("WorkOrderTemplate/Update", content);

        Assert.True(httpResponseMessage.IsSuccessStatusCode, "The operation should have been successful."); //The operation must have been successful.
        Assert.NotEqual(HttpStatusCode.NoContent, httpResponseMessage.StatusCode); //Content must have been returned.

        string html = await httpResponseMessage.Content.ReadAsStringAsync();

        //HTML must have been returned.
        Assert.NotEmpty(html);
        Assert.StartsWith(DocTypeHtmlTag, html);
        Assert.Contains("<span class=\"text-danger field-validation-error\" data-valmsg-for=\"Name\" data-valmsg-replace=\"true\">The Name field is required.</span>", html);
    }

    /// <summary>
    /// The method verifies the work order template controller will return a not found when it receives an update request 
    /// by the user and the work order doesn't exist.
    /// </summary>
    /// <returns>A Task for the async.</returns>
    [Fact]
    public async Task VerifyUpdateWorkOrderTemplateNotFound()
    {
        Dictionary<string, string> formValues = new()
        {
            { "daysDueFromCreation", "0" },
            { "description", string.Empty },
            { "name", "a name" },
            { "otherTypeOfService", string.Empty },
            { "priority", ((int)WorkOrderPriority.Normal).ToString() },
            { "serviceType", ((int)WorkOrderServiceType.Inspection).ToString() },
        };
        FormUrlEncodedContent content = new(formValues);

        HttpClient httpClient = _factory.CreateClient();
        HttpResponseMessage httpResponseMessage = await httpClient.PostAsync("WorkOrderTemplate/Update/999999", content);

        Assert.True(httpResponseMessage.IsSuccessStatusCode, "The operation should have been successful."); //The operation must have been successful.
        Assert.NotEqual(HttpStatusCode.NoContent, httpResponseMessage.StatusCode); //Content must have been returned.

        string html = await httpResponseMessage.Content.ReadAsStringAsync();

        //HTML must have been returned.
        Assert.NotEmpty(html);
        Assert.StartsWith(DocTypeHtmlTag, html);
        Assert.Contains(NotFoundHtmlSearchTag, html);
    }

    /// <summary>
    /// The method verifies the work order template controller will return a conflict when two users try to update the same
    /// work order template.
    /// </summary>
    /// <returns>A Task for the async.</returns>
    [Fact]
    public async Task VerifyUpdateWorkOrderTemplateOldDataConflict()
    {
        HttpClient httpClient = _factory.CreateClient();
        long? id = await CreateWorkOrderTemplateAsync(httpClient, "Update Work Order Old Data Conflict Test");

        if (id is null)
        {
            Assert.Fail("Failed to create a work order template for the test.");
        }

        Dictionary<string, string> formValues = new()
        {
            { "daysDueFromCreation", "0" },
            { "description", string.Empty },
            { "name", "Update Work Order Old Data Conflict Test" },
            { "integer64ID", id.Value.ToString() },
            { "otherTypeOfService", string.Empty },
            { "priority", ((int)WorkOrderPriority.Low).ToString() },
            { "serviceType", ((int)WorkOrderServiceType.Inspection).ToString() },
        };
        FormUrlEncodedContent content = new(formValues);

        HttpResponseMessage httpResponseMessage = await httpClient.PostAsync("WorkOrderTemplate/Update", content);

        Assert.True(httpResponseMessage.IsSuccessStatusCode, "The operation should have been successful."); //The operation must have been successful.
        Assert.NotEqual(HttpStatusCode.NoContent, httpResponseMessage.StatusCode); //Content must have been returned.

        string html = await httpResponseMessage.Content.ReadAsStringAsync();

        //HTML must have been returned.
        Assert.NotEmpty(html);
        Assert.StartsWith(DocTypeHtmlTag, html);
        Assert.DoesNotContain(ConflictHtmlSearchTag, html);
        Assert.DoesNotContain(ErrorHtmlSearchTag, html);
        Assert.DoesNotContain(NotFoundHtmlSearchTag, html);
        Assert.Contains("Update Work Order Old Data Conflict Test", html);

        httpResponseMessage = await httpClient.PostAsync("WorkOrderTemplate/Update", content);

        Assert.True(httpResponseMessage.IsSuccessStatusCode, "The operation should have been successful."); //The operation must have been successful.
        Assert.NotEqual(HttpStatusCode.NoContent, httpResponseMessage.StatusCode); //Content must have been returned.

        html = await httpResponseMessage.Content.ReadAsStringAsync();

        //HTML must have been returned.
        Assert.NotEmpty(html);
        Assert.StartsWith(DocTypeHtmlTag, html);
        Assert.Contains(ConflictHtmlSearchTag, html);
    }
}
