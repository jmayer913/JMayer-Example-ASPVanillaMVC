using JMayer.Example.ASPVanillaMVC.Models;
using System.Net;

namespace TestProject;

/// <summary>
/// The static class contains helper methods for data creation.
/// </summary>
internal static class DataHelper
{
    /// <summary>
    /// The method creates a work order template on the remote web server.
    /// </summary>
    /// <param name="httpClient">Used to communicate with the web server.</param>
    /// <param name="name">The name of the work order template.</param>
    /// <returns>The created work order template.</returns>
    public static async Task<long?> CreateWorkOrderTemplateAsync(HttpClient httpClient, string name)
    {
        FormUrlEncodedContent content = CreateWorkOrderTemplateFormUrlEncodedContent(name, string.Empty, WorkOrderServiceType.Inspection, string.Empty, WorkOrderPriority.Normal, 0);
        HttpResponseMessage httpResponseMessage = await httpClient.PostAsync("WorkOrderTemplate/Create", content, TestContext.Current.CancellationToken);

        if (httpResponseMessage.IsSuccessStatusCode is false || httpResponseMessage.StatusCode is HttpStatusCode.NoContent)
        {
            return null;
        }

        string html = await httpResponseMessage.Content.ReadAsStringAsync(TestContext.Current.CancellationToken);

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
    /// The method return the form url encoded content for a work order template.
    /// </summary>
    /// <param name="name">The friendly name for the work order.</param>
    /// <param name="description">A description for the work order.</param>
    /// <param name="serviceType">The type of service for the work order.</param>
    /// <param name="otherTypeOfService">A custom description for the type when the standard (Inspection, Routine & Reactive) does not apply.</param>
    /// <param name="priority">The priority for the work order.</param>
    /// <param name="daysDueFromCreation">The number of days the work order will be due based on when its created.</param>
    /// <returns>The form url encoded content for the work order template.</returns>
    public static FormUrlEncodedContent CreateWorkOrderTemplateFormUrlEncodedContent(string name, string description, WorkOrderServiceType serviceType, string? otherTypeOfService, WorkOrderPriority priority, int daysDueFromCreation)
        => CreateWorkOrderTemplateFormUrlEncodedContent(name, description, serviceType, otherTypeOfService, priority, daysDueFromCreation, 0);

    /// <summary>
    /// The method return the form url encoded content for a work order template.
    /// </summary>
    /// <param name="name">The friendly name for the work order.</param>
    /// <param name="description">A description for the work order.</param>
    /// <param name="serviceType">The type of service for the work order.</param>
    /// <param name="otherTypeOfService">A custom description for the type when the standard (Inspection, Routine & Reactive) does not apply.</param>
    /// <param name="priority">The priority for the work order.</param>
    /// <param name="daysDueFromCreation">The number of days the work order will be due based on when its created.</param>
    /// <param name="id">The record identifier.</param>
    /// <returns>The form url encoded content for the work order template.</returns>
    public static FormUrlEncodedContent CreateWorkOrderTemplateFormUrlEncodedContent(string name, string description, WorkOrderServiceType serviceType, string? otherTypeOfService, WorkOrderPriority priority, int daysDueFromCreation, long id)
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

        if (id > 0)
        {
            formValues.Add("integer64ID", id.ToString());
        }

        return new FormUrlEncodedContent(formValues);
    }

    /// <summary>
    /// The method return the form url encoded content for a work order template schedule.
    /// </summary>
    /// <param name="id">The record identifier.</param>
    /// <param name="scheduleType">The frequency for the schedule.</param>
    /// <param name="enabled">Whether or not the scheduler considers this work order template and schedule.</param>
    /// <returns>The form url encoded content for the work order template schedule.</returns>
    public static FormUrlEncodedContent CreateWorkOrderTemplateScheduleFormUrlEncodedContent(long id, WorkOrderTemplateScheduleType scheduleType, bool enabled)
        => CreateWorkOrderTemplateScheduleFormUrlEncodedContent(id, scheduleType, enabled, DateTime.Today, null);

    /// <summary>
    /// The method return the form url encoded content for a work order template schedule.
    /// </summary>
    /// <param name="id">The record identifier.</param>
    /// <param name="scheduleType">The frequency for the schedule.</param>
    /// <param name="enabled">Whether or not the scheduler considers this work order template and schedule.</param>
    /// <param name="startDate"></param>
    /// <param name="endDate"></param>
    /// <returns>The form url encoded content for the work order template schedule.</returns>
    public static FormUrlEncodedContent CreateWorkOrderTemplateScheduleFormUrlEncodedContent(long id, WorkOrderTemplateScheduleType scheduleType, bool enabled, DateTime startDate, DateTime? endDate)
    {
        Dictionary<string, string> formValues = new()
        {
            { "endDate", endDate is not null ? endDate.Value.ToShortDateString() : string.Empty },
            { "integer64ID", id.ToString() },
            { "isEnabled", enabled.ToString() },
            { "ownerInteger64ID", id.ToString() },
            { "scheduleType", ((int)scheduleType).ToString() },
            { "startDate", startDate.ToShortDateString() },
        };
        return new FormUrlEncodedContent(formValues);
    }
}
