using JMayer.Data.Database.DataLayer.MemoryStorage;
using JMayer.Example.ASPVanillaMVC.Models;

namespace JMayer.Example.ASPVanillaMVC.DataLayers;

/// <summary>
/// The class manages CRUD interactions with the database for a work order template.
/// </summary>
public class WorkOrderTemplateDataLayer : StandardCRUDDataLayer<WorkOrderTemplate>, IWorkOrderTemplateDataLayer
{
    /// <summary>
    /// The default constructor.
    /// </summary>
    public WorkOrderTemplateDataLayer()
    {
        IsOldDataObjectDetectionEnabled = true;
        IsUniqueNameRequired = true;
    }
}
