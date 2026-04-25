using JMayer.Example.ASPVanillaMVC.Models;
using Microsoft.AspNetCore.Mvc.Testing;
using System.Net;
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
    /// The dependency injection constructor.
    /// </summary>
    /// <param name="factory">The factory for the web application.</param>
    public WorkOrderTemplateUnitTest(WebApplicationFactory<Program> factory) => _factory = factory;

    /// <summary>
    /// The method verifies the work order template controller can return the add view when requested by the user.
    /// </summary>
    /// <returns>A Task for the async.</returns>
    [Fact]
    public async Task VerifyAddView()
    {
        HttpClient client = _factory.CreateClient();
        HttpResponseMessage httpResponseMessage = await client.GetAsync("WorkOrderTemplate/AddView", CancellationToken.None);

        Assert.True(httpResponseMessage.IsSuccessStatusCode, Constants.NonSuccessfulResponseFailureMessage); //The operation must have been successful.
        Assert.NotEqual(HttpStatusCode.NoContent, httpResponseMessage.StatusCode); //Content must have been returned.

        string html = await httpResponseMessage.Content.ReadAsStringAsync(CancellationToken.None);

        //HTML must have been returned.
        Assert.NotEmpty(html);
        Assert.StartsWith(Constants.DocTypeHtmlTag, html);
        Assert.DoesNotContain(Constants.ErrorHtmlSearchTag, html);
    }

    /// <summary>
    /// The method verifies the work order template controller can return the confirm create work order view when requested.
    /// </summary>
    /// <returns>A Task for the async.</returns>
    [Fact]
    public async Task VerifyConfirmCreateWorkOrderView()
    {
        HttpClient client = _factory.CreateClient();
        HttpResponseMessage httpResponseMessage = await client.GetAsync("WorkOrderTemplate/ConfirmCreateWorkOrderView/1", CancellationToken.None);

        Assert.True(httpResponseMessage.IsSuccessStatusCode, Constants.NonSuccessfulResponseFailureMessage); //The operation must have been successful.
        Assert.NotEqual(HttpStatusCode.NoContent, httpResponseMessage.StatusCode); //Content must have been returned.

        string html = await httpResponseMessage.Content.ReadAsStringAsync(CancellationToken.None);

        //HTML must have been returned.
        Assert.NotEmpty(html);
        Assert.StartsWith(Constants.DocTypeHtmlTag, html);
        Assert.DoesNotContain(Constants.ErrorHtmlSearchTag, html);
        Assert.DoesNotContain(Constants.NotFoundHtmlSearchTag, html);
    }

    /// <summary>
    /// The method verifies the work order template controller returns the not found view when the requested id does not exist.
    /// </summary>
    /// <returns>A Task for the async.</returns>
    [Fact]
    public async Task VerifyConfirmCreateWorkOrderViewNotFound()
    {
        HttpClient client = _factory.CreateClient();
        HttpResponseMessage httpResponseMessage = await client.GetAsync("WorkOrderTemplate/ConfirmCreateWorkOrderView/999999", CancellationToken.None);

        Assert.True(httpResponseMessage.IsSuccessStatusCode, Constants.NonSuccessfulResponseFailureMessage); //The operation must have been successful.
        Assert.NotEqual(HttpStatusCode.NoContent, httpResponseMessage.StatusCode); //Content must have been returned.

        string html = await httpResponseMessage.Content.ReadAsStringAsync(CancellationToken.None);

        //HTML must have been returned.
        Assert.NotEmpty(html);
        Assert.StartsWith(Constants.DocTypeHtmlTag, html);
        Assert.Contains(Constants.NotFoundHtmlSearchTag, html);
    }

    /// <summary>
    /// The method verifies the work order template controller can return the confirm create work order view when requested.
    /// </summary>
    /// <returns>A Task for the async.</returns>
    [Fact]
    public async Task VerifyCreateWorkOrder()
    {
        HttpClient client = _factory.CreateClient();
        HttpResponseMessage httpResponseMessage = await client.GetAsync("WorkOrderTemplate/CreateWorkOrder/1", CancellationToken.None);

        Assert.True(httpResponseMessage.IsSuccessStatusCode, Constants.NonSuccessfulResponseFailureMessage); //The operation must have been successful.
        Assert.NotEqual(HttpStatusCode.NoContent, httpResponseMessage.StatusCode); //Content must have been returned.

        string html = await httpResponseMessage.Content.ReadAsStringAsync(CancellationToken.None);

        //HTML must have been returned.
        Assert.NotEmpty(html);
        Assert.StartsWith(Constants.DocTypeHtmlTag, html);
        Assert.DoesNotContain(Constants.ErrorHtmlSearchTag, html);
    }

    /// <summary>
    /// The method verifies the work order template controller can create a work order template when requested by the user.
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
        HttpClient httpClient = _factory.CreateClient();
        FormUrlEncodedContent content = DataHelper.CreateWorkOrderTemplateFormUrlEncodedContent(name, description, serviceType, otherTypeOfService, priority, daysDueFromCreation);
        HttpResponseMessage httpResponseMessage = await httpClient.PostAsync("WorkOrderTemplate/Create", content, CancellationToken.None);

        Assert.True(httpResponseMessage.IsSuccessStatusCode, Constants.NonSuccessfulResponseFailureMessage); //The operation must have been successful.
        Assert.NotEqual(HttpStatusCode.NoContent, httpResponseMessage.StatusCode); //Content must have been returned.

        string html = await httpResponseMessage.Content.ReadAsStringAsync(CancellationToken.None);

        //HTML must have been returned.
        Assert.NotEmpty(html);
        Assert.StartsWith(Constants.DocTypeHtmlTag, html);
        Assert.DoesNotContain(Constants.ErrorHtmlSearchTag, html);
        Assert.DoesNotContain(Constants.NotFoundHtmlSearchTag, html);
        Assert.Contains(name, html);
    }

    /// <summary>
    /// The method verifies the work order template controller will return html with a validation error when it receives a create requested 
    /// but another work order template has the same name.
    /// </summary>
    /// <returns>A Task for the async.</returns>
    [Fact]
    public async Task VerifyCreateWorkOrderTemplateDuplicateFailure()
    {
        HttpClient httpClient = _factory.CreateClient();
        long? id = await DataHelper.CreateWorkOrderTemplateAsync(httpClient, "Create Work Order Template Duplicate Test");

        if (id is null)
        {
            Assert.Fail(Constants.CreateTemplateFailureMessage);
        }

        FormUrlEncodedContent content = DataHelper.CreateWorkOrderTemplateFormUrlEncodedContent("Create Work Order Template Duplicate Test", string.Empty, WorkOrderServiceType.Inspection, string.Empty, WorkOrderPriority.Normal, 0);
        HttpResponseMessage httpResponseMessage = await httpClient.PostAsync("WorkOrderTemplate/Create", content, CancellationToken.None);

        Assert.True(httpResponseMessage.IsSuccessStatusCode, Constants.NonSuccessfulResponseFailureMessage); //The operation must have been successful.
        Assert.NotEqual(HttpStatusCode.NoContent, httpResponseMessage.StatusCode); //Content must have been returned.

        
        string html = await httpResponseMessage.Content.ReadAsStringAsync(CancellationToken.None);

        //HTML must have been returned.
        Assert.NotEmpty(html);
        Assert.StartsWith(Constants.DocTypeHtmlTag, html);
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
        HttpClient httpClient = _factory.CreateClient();
        FormUrlEncodedContent content = DataHelper.CreateWorkOrderTemplateFormUrlEncodedContent(string.Empty, string.Empty, WorkOrderServiceType.Inspection, string.Empty, WorkOrderPriority.Normal, 0);
        HttpResponseMessage httpResponseMessage = await httpClient.PostAsync("WorkOrderTemplate/Create", content, CancellationToken.None);

        Assert.True(httpResponseMessage.IsSuccessStatusCode, Constants.NonSuccessfulResponseFailureMessage); //The operation must have been successful.
        Assert.NotEqual(HttpStatusCode.NoContent, httpResponseMessage.StatusCode); //Content must have been returned.

        string html = await httpResponseMessage.Content.ReadAsStringAsync(CancellationToken.None);

        //HTML must have been returned.
        Assert.NotEmpty(html);
        Assert.StartsWith(Constants.DocTypeHtmlTag, html);
        Assert.Contains("<span class=\"text-danger field-validation-error\" data-valmsg-for=\"Name\" data-valmsg-replace=\"true\">The Name field is required.</span>", html);
    }

    /// <summary>
    /// The method verifies the work order template controller will return html with a validation error when it receives a create requested 
    /// by the user and the work order templatels service type is other and other type of service is empty.
    /// </summary>
    /// <returns>A Task for the async.</returns>
    [Fact]
    public async Task VerifyCreateWorkOrderTemplateOtherTypeOfServiceRequiredValidationFailure()
    {
        HttpClient httpClient = _factory.CreateClient();
        FormUrlEncodedContent content = DataHelper.CreateWorkOrderTemplateFormUrlEncodedContent(nameof(VerifyCreateWorkOrderTemplateOtherTypeOfServiceRequiredValidationFailure), string.Empty, WorkOrderServiceType.Other, string.Empty, WorkOrderPriority.Normal, 0);
        HttpResponseMessage httpResponseMessage = await httpClient.PostAsync("WorkOrderTemplate/Create", content, CancellationToken.None);

        Assert.True(httpResponseMessage.IsSuccessStatusCode, Constants.NonSuccessfulResponseFailureMessage); //The operation must have been successful.
        Assert.NotEqual(HttpStatusCode.NoContent, httpResponseMessage.StatusCode); //Content must have been returned.

        string html = await httpResponseMessage.Content.ReadAsStringAsync(CancellationToken.None);

        //HTML must have been returned.
        Assert.NotEmpty(html);
        Assert.StartsWith(Constants.DocTypeHtmlTag, html);
        Assert.Contains("<span class=\"text-danger field-validation-error\" data-valmsg-for=\"OtherTypeOfService\" data-valmsg-replace=\"true\">The OtherTypeOfService field is required.</span>", html);
    }

    /// <summary>
    /// The method verifies the work order template controller can delete a work order template when requested by the user.
    /// </summary>
    /// <returns></returns>
    [Fact]
    public async Task VerifyDeleteWorkOrderTemplate()
    {
        HttpClient httpClient = _factory.CreateClient();
        long? id = await DataHelper.CreateWorkOrderTemplateAsync(httpClient, "Delete Work Order Template Test");

        if (id is null)
        {
            Assert.Fail(Constants.CreateTemplateFailureMessage);
        }

        HttpResponseMessage httpResponseMessage = await httpClient.PostAsync($"WorkOrderTemplate/Delete/{id}", new StringContent(string.Empty), CancellationToken.None);

        Assert.True(httpResponseMessage.IsSuccessStatusCode, Constants.NonSuccessfulResponseFailureMessage); //The operation must have been successful.
        Assert.NotEqual(HttpStatusCode.NoContent, httpResponseMessage.StatusCode); //Content must have been returned.

        string html = await httpResponseMessage.Content.ReadAsStringAsync(CancellationToken.None);

        //HTML must have been returned.
        Assert.NotEmpty(html);
        Assert.StartsWith(Constants.DocTypeHtmlTag, html);
        Assert.DoesNotContain(Constants.ConflictHtmlSearchTag, html);
        Assert.DoesNotContain(Constants.ErrorHtmlSearchTag, html);
        Assert.DoesNotContain(Constants.NotFoundHtmlSearchTag, html);
    }

    /// <summary>
    /// The method verifies the work order template controller can delete a work order when requested by the user.
    /// </summary>
    /// <returns></returns>
    [Fact]
    public async Task VerifyDeleteWorkOrderTemplateNotFound()
    {
        HttpClient httpClient = _factory.CreateClient();
        HttpResponseMessage httpResponseMessage = await httpClient.PostAsync("WorkOrderTemplate/Delete/999999", new StringContent(string.Empty), CancellationToken.None);

        Assert.True(httpResponseMessage.IsSuccessStatusCode, Constants.NonSuccessfulResponseFailureMessage); //The operation must have been successful.
        Assert.NotEqual(HttpStatusCode.NoContent, httpResponseMessage.StatusCode); //Content must have been returned.

        string html = await httpResponseMessage.Content.ReadAsStringAsync(CancellationToken.None);

        //HTML must have been returned.
        Assert.NotEmpty(html);
        Assert.StartsWith(Constants.DocTypeHtmlTag, html);
        Assert.Contains(Constants.NotFoundHtmlSearchTag, html);
    }

    /// <summary>
    /// The method verifies the work order template controller can return the delete view when requested by the user.
    /// </summary>
    /// <returns>A Task for the async.</returns>
    [Fact]
    public async Task VerifyDeleteView()
    {
        HttpClient client = _factory.CreateClient();
        HttpResponseMessage httpResponseMessage = await client.GetAsync("WorkOrderTemplate/DeleteView/1", CancellationToken.None);

        Assert.True(httpResponseMessage.IsSuccessStatusCode, Constants.NonSuccessfulResponseFailureMessage); //The operation must have been successful.
        Assert.NotEqual(HttpStatusCode.NoContent, httpResponseMessage.StatusCode); //Content must have been returned.

        string html = await httpResponseMessage.Content.ReadAsStringAsync(CancellationToken.None);

        //HTML must have been returned.
        Assert.NotEmpty(html); 
        Assert.StartsWith(Constants.DocTypeHtmlTag, html);
        Assert.DoesNotContain(Constants.ErrorHtmlSearchTag, html);
        Assert.DoesNotContain(Constants.NotFoundHtmlSearchTag, html);
    }

    /// <summary>
    /// The method verifies the work order template controller returns the not found view when the requested id does not exist.
    /// </summary>
    /// <returns>A Task for the async.</returns>
    [Fact]
    public async Task VerifyDeleteViewNotFound()
    {
        HttpClient client = _factory.CreateClient();
        HttpResponseMessage httpResponseMessage = await client.GetAsync("WorkOrderTemplate/DeleteView/999999", CancellationToken.None);

        Assert.True(httpResponseMessage.IsSuccessStatusCode, Constants.NonSuccessfulResponseFailureMessage); //The operation must have been successful.
        Assert.NotEqual(HttpStatusCode.NoContent, httpResponseMessage.StatusCode); //Content must have been returned.

        string html = await httpResponseMessage.Content.ReadAsStringAsync(CancellationToken.None);

        //HTML must have been returned.
        Assert.NotEmpty(html);
        Assert.StartsWith(Constants.DocTypeHtmlTag, html);
        Assert.Contains(Constants.NotFoundHtmlSearchTag, html);
    }

    /// <summary>
    /// The method verifies the work order template controller can return the edit view when requested.
    /// </summary>
    /// <returns>A Task for the async.</returns>
    [Fact]
    public async Task VerifyEditView()
    {
        HttpClient client = _factory.CreateClient();
        HttpResponseMessage httpResponseMessage = await client.GetAsync("WorkOrderTemplate/EditView/1", CancellationToken.None);

        Assert.True(httpResponseMessage.IsSuccessStatusCode, Constants.NonSuccessfulResponseFailureMessage); //The operation must have been successful.
        Assert.NotEqual(HttpStatusCode.NoContent, httpResponseMessage.StatusCode); //Content must have been returned.

        string html = await httpResponseMessage.Content.ReadAsStringAsync(CancellationToken.None);

        //HTML must have been returned.
        Assert.NotEmpty(html);
        Assert.StartsWith(Constants.DocTypeHtmlTag, html);
        Assert.DoesNotContain(Constants.ErrorHtmlSearchTag, html);
        Assert.DoesNotContain(Constants.NotFoundHtmlSearchTag, html);
    }

    /// <summary>
    /// The method verifies the work order template controller returns the not found view when the requested id does not exist.
    /// </summary>
    /// <returns>A Task for the async.</returns>
    [Fact]
    public async Task VerifyEditViewNotFound()
    {
        HttpClient client = _factory.CreateClient();
        HttpResponseMessage httpResponseMessage = await client.GetAsync("WorkOrderTemplate/EditView/999999", CancellationToken.None);

        Assert.True(httpResponseMessage.IsSuccessStatusCode, Constants.NonSuccessfulResponseFailureMessage); //The operation must have been successful.
        Assert.NotEqual(HttpStatusCode.NoContent, httpResponseMessage.StatusCode); //Content must have been returned.

        string html = await httpResponseMessage.Content.ReadAsStringAsync(CancellationToken.None);

        //HTML must have been returned.
        Assert.NotEmpty(html);
        Assert.StartsWith(Constants.DocTypeHtmlTag, html);
        Assert.Contains(Constants.NotFoundHtmlSearchTag, html);
    }

    /// <summary>
    /// The method verifies the work order template controller can return the index view.
    /// </summary>
    /// <returns>A Task for the async.</returns>
    [Fact]
    public async Task VerifyIndexView()
    {
        HttpClient client = _factory.CreateClient();
        HttpResponseMessage httpResponseMessage = await client.GetAsync("WorkOrderTemplate/Index", CancellationToken.None);

        Assert.True(httpResponseMessage.IsSuccessStatusCode, Constants.NonSuccessfulResponseFailureMessage); //The operation must have been successful.
        Assert.NotEqual(HttpStatusCode.NoContent, httpResponseMessage.StatusCode); //Content must have been returned.

        string html = await httpResponseMessage.Content.ReadAsStringAsync(CancellationToken.None);

        //HTML must have been returned.
        Assert.NotEmpty(html);
        Assert.StartsWith(Constants.DocTypeHtmlTag, html);
        Assert.DoesNotContain(Constants.ErrorHtmlSearchTag, html);
    }

    /// <summary>
    /// The method verifies the work order template controller can update a work order template when requested by the user.
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
        long? id = await DataHelper.CreateWorkOrderTemplateAsync(client, originalName);

        if (id is null)
        {
            Assert.Fail(Constants.CreateTemplateFailureMessage);
        }

        HttpClient httpClient = _factory.CreateClient();
        FormUrlEncodedContent content = DataHelper.CreateWorkOrderTemplateFormUrlEncodedContent(newName, description, serviceType, otherTypeOfService, priority, daysDueFromCreation, id.Value);
        HttpResponseMessage httpResponseMessage = await httpClient.PostAsync("WorkOrderTemplate/Update", content, CancellationToken.None);

        Assert.True(httpResponseMessage.IsSuccessStatusCode, Constants.NonSuccessfulResponseFailureMessage); //The operation must have been successful.
        Assert.NotEqual(HttpStatusCode.NoContent, httpResponseMessage.StatusCode); //Content must have been returned.

        string html = await httpResponseMessage.Content.ReadAsStringAsync(CancellationToken.None);

        //HTML must have been returned.
        Assert.NotEmpty(html);
        Assert.StartsWith(Constants.DocTypeHtmlTag, html);
        Assert.DoesNotContain(Constants.ConflictHtmlSearchTag, html);
        Assert.DoesNotContain(Constants.ErrorHtmlSearchTag, html);
        Assert.DoesNotContain(Constants.NotFoundHtmlSearchTag, html);
        Assert.Contains(newName, html);
    }

    /// <summary>
    /// The method verifies the work order template controller will return html with a validation error when it receives a create requested 
    /// but another work order template has the same name.
    /// </summary>
    /// <returns>A Task for the async.</returns>
    [Fact]
    public async Task VerifyUpdateWorkOrderTemplateDuplicateFailure()
    {
        HttpClient httpClient = _factory.CreateClient();
        long? id = await DataHelper.CreateWorkOrderTemplateAsync(httpClient, "Update Work Order Template Duplicate Test 1");

        if (id is null)
        {
            Assert.Fail(Constants.CreateTemplateFailureMessage);
        }

        id = await DataHelper.CreateWorkOrderTemplateAsync(httpClient, "Update Work Order Template Duplicate Test 2");

        if (id is null)
        {
            Assert.Fail(Constants.CreateTemplateFailureMessage);
        }

        FormUrlEncodedContent content = DataHelper.CreateWorkOrderTemplateFormUrlEncodedContent("Update Work Order Template Duplicate Test 1", string.Empty, WorkOrderServiceType.Inspection, string.Empty, WorkOrderPriority.Normal, 0, id.Value);
        HttpResponseMessage httpResponseMessage = await httpClient.PostAsync("WorkOrderTemplate/Create", content, CancellationToken.None);

        Assert.True(httpResponseMessage.IsSuccessStatusCode, Constants.NonSuccessfulResponseFailureMessage); //The operation must have been successful.
        Assert.NotEqual(HttpStatusCode.NoContent, httpResponseMessage.StatusCode); //Content must have been returned.

        string html = await httpResponseMessage.Content.ReadAsStringAsync(CancellationToken.None);

        //HTML must have been returned.
        Assert.NotEmpty(html);
        Assert.StartsWith(Constants.DocTypeHtmlTag, html);
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
        HttpClient httpClient = _factory.CreateClient();
        FormUrlEncodedContent content = DataHelper.CreateWorkOrderTemplateFormUrlEncodedContent(string.Empty, string.Empty, WorkOrderServiceType.Inspection, string.Empty, WorkOrderPriority.Normal, 0, 1);
        HttpResponseMessage httpResponseMessage = await httpClient.PostAsync("WorkOrderTemplate/Update", content, CancellationToken.None);

        Assert.True(httpResponseMessage.IsSuccessStatusCode, Constants.NonSuccessfulResponseFailureMessage); //The operation must have been successful.
        Assert.NotEqual(HttpStatusCode.NoContent, httpResponseMessage.StatusCode); //Content must have been returned.

        string html = await httpResponseMessage.Content.ReadAsStringAsync(CancellationToken.None);

        //HTML must have been returned.
        Assert.NotEmpty(html);
        Assert.StartsWith(Constants.DocTypeHtmlTag, html);
        Assert.Contains("<span class=\"text-danger field-validation-error\" data-valmsg-for=\"Name\" data-valmsg-replace=\"true\">The Name field is required.</span>", html);
    }

    /// <summary>
    /// The method verifies the work order template controller will return html with a validation error when it receives an update requested 
    /// by the user and the work order template's service type is other and other type of service is empty.
    /// </summary>
    /// <returns>A Task for the async.</returns>
    [Fact]
    public async Task VerifyUpdateWorkOrderTemplateOtherTypeOfServiceRequiredValidationFailure()
    {
        HttpClient httpClient = _factory.CreateClient();
        FormUrlEncodedContent content = DataHelper.CreateWorkOrderTemplateFormUrlEncodedContent(nameof(VerifyCreateWorkOrderTemplateOtherTypeOfServiceRequiredValidationFailure), string.Empty, WorkOrderServiceType.Other, string.Empty, WorkOrderPriority.Normal, 0, 1);
        HttpResponseMessage httpResponseMessage = await httpClient.PostAsync("WorkOrderTemplate/Update", content, CancellationToken.None);

        Assert.True(httpResponseMessage.IsSuccessStatusCode, Constants.NonSuccessfulResponseFailureMessage); //The operation must have been successful.
        Assert.NotEqual(HttpStatusCode.NoContent, httpResponseMessage.StatusCode); //Content must have been returned.

        string html = await httpResponseMessage.Content.ReadAsStringAsync(CancellationToken.None);

        //HTML must have been returned.
        Assert.NotEmpty(html);
        Assert.StartsWith(Constants.DocTypeHtmlTag, html);
        Assert.Contains("<span class=\"text-danger field-validation-error\" data-valmsg-for=\"OtherTypeOfService\" data-valmsg-replace=\"true\">The OtherTypeOfService field is required.</span>", html);
    }

    /// <summary>
    /// The method verifies the work order template controller will return a not found when it receives an update request 
    /// by the user and the work order template doesn't exist.
    /// </summary>
    /// <returns>A Task for the async.</returns>
    [Fact]
    public async Task VerifyUpdateWorkOrderTemplateNotFound()
    {
        HttpClient httpClient = _factory.CreateClient();
        FormUrlEncodedContent content = DataHelper.CreateWorkOrderTemplateFormUrlEncodedContent("a name", string.Empty, WorkOrderServiceType.Inspection, string.Empty, WorkOrderPriority.Normal, 0, 999999);
        HttpResponseMessage httpResponseMessage = await httpClient.PostAsync("WorkOrderTemplate/Update", content, CancellationToken.None);

        Assert.True(httpResponseMessage.IsSuccessStatusCode, Constants.NonSuccessfulResponseFailureMessage); //The operation must have been successful.
        Assert.NotEqual(HttpStatusCode.NoContent, httpResponseMessage.StatusCode); //Content must have been returned.

        string html = await httpResponseMessage.Content.ReadAsStringAsync(CancellationToken.None);

        //HTML must have been returned.
        Assert.NotEmpty(html);
        Assert.StartsWith(Constants.DocTypeHtmlTag, html);
        Assert.Contains(Constants.NotFoundHtmlSearchTag, html);
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
        long? id = await DataHelper.CreateWorkOrderTemplateAsync(httpClient, "Update Work Order Template Old Data Conflict Test");

        if (id is null)
        {
            Assert.Fail(Constants.CreateTemplateFailureMessage);
        }

        FormUrlEncodedContent content = DataHelper.CreateWorkOrderTemplateFormUrlEncodedContent("Update Work Order Template Old Data Conflict Test", string.Empty, WorkOrderServiceType.Inspection, string.Empty, WorkOrderPriority.Normal, 0, id.Value);
        HttpResponseMessage httpResponseMessage = await httpClient.PostAsync("WorkOrderTemplate/Update", content, CancellationToken.None);

        Assert.True(httpResponseMessage.IsSuccessStatusCode, Constants.NonSuccessfulResponseFailureMessage); //The operation must have been successful.
        Assert.NotEqual(HttpStatusCode.NoContent, httpResponseMessage.StatusCode); //Content must have been returned.

        string html = await httpResponseMessage.Content.ReadAsStringAsync(CancellationToken.None);

        //HTML must have been returned.
        Assert.NotEmpty(html);
        Assert.StartsWith(Constants.DocTypeHtmlTag, html);
        Assert.DoesNotContain(Constants.ConflictHtmlSearchTag, html);
        Assert.DoesNotContain(Constants.ErrorHtmlSearchTag, html);
        Assert.DoesNotContain(Constants.NotFoundHtmlSearchTag, html);
        Assert.Contains("Update Work Order Template Old Data Conflict Test", html);

        httpResponseMessage = await httpClient.PostAsync("WorkOrderTemplate/Update", content, CancellationToken.None);

        Assert.True(httpResponseMessage.IsSuccessStatusCode, Constants.NonSuccessfulResponseFailureMessage); //The operation must have been successful.
        Assert.NotEqual(HttpStatusCode.NoContent, httpResponseMessage.StatusCode); //Content must have been returned.

        html = await httpResponseMessage.Content.ReadAsStringAsync(CancellationToken.None);

        //HTML must have been returned.
        Assert.NotEmpty(html);
        Assert.StartsWith(Constants.DocTypeHtmlTag, html);
        Assert.Contains(Constants.ConflictHtmlSearchTag, html);
    }
}
