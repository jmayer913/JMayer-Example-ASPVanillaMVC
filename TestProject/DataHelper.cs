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
}
