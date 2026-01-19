using JMayer.Example.ASPVanillaMVC.Models;

namespace TestProject.Test.EqualityComparer;

/// <summary>
/// The class manages testing the work order template equality comparer.
/// </summary>
public class WorkOrderTemplateUnitTest
{
    /// <summary>
    /// The constant for the days due from creation.
    /// </summary>
    private const int DaysDueFromCreation = 7;

    /// <summary>
    /// The constant for the description.
    /// </summary>
    private const string Description = "A Description";

    /// <summary>
    /// The constant for the ID.
    /// </summary>
    private const long ID = 1;

    /// <summary>
    /// The constant for the name.
    /// </summary>
    private const string Name = "A Name";

    /// <summary>
    /// The constant for the other type of service.
    /// </summary>
    private const string OtherTypeOfService = "Another Type of Service";

    /// <summary>
    /// The method verifies equality failure when two nulls are compared.
    /// </summary>
    [Fact]
    public void VerifyFailureBothNull() => Assert.False(new WorkOrderTemplateEqualityComparer().Equals(null, null));

    /// <summary>
    /// The method verifies equality failure when the CreatedOn property is different between the two objects.
    /// </summary>
    [Fact]
    public void VerifyFailureCreatedOn()
    {
        WorkOrderTemplate workOrderTemplate1 = new()
        {
            CreatedOn = DateTime.Now,
        };
        WorkOrderTemplate workOrderTemplate2 = new();

        Assert.False(new WorkOrderTemplateEqualityComparer().Equals(workOrderTemplate1, workOrderTemplate2));
    }

    /// <summary>
    /// The method verifies equality failure when the DaysDueFromCreation property is different between the two objects.
    /// </summary>
    [Fact]
    public void VerifyFailureDaysDueFromCreation()
    {
        WorkOrderTemplate workOrderTemplate1 = new()
        {
            DaysDueFromCreation = DaysDueFromCreation,
        };
        WorkOrderTemplate workOrderTemplate2 = new();

        Assert.False(new WorkOrderTemplateEqualityComparer().Equals(workOrderTemplate1, workOrderTemplate2));
    }

    /// <summary>
    /// The method verifies equality failure when the Description property is different between the two objects.
    /// </summary>
    [Fact]
    public void VerifyFailureDescription()
    {
        WorkOrderTemplate workOrderTemplate1 = new()
        {
            Description = Description,
        };
        WorkOrderTemplate workOrderTemplate2 = new();

        Assert.False(new WorkOrderTemplateEqualityComparer().Equals(workOrderTemplate1, workOrderTemplate2));
    }

    /// <summary>
    /// The method verifies equality failure when the ID property is different between the two objects.
    /// </summary>
    [Fact]
    public void VerifyFailureID()
    {
        WorkOrderTemplate workOrderTemplate1 = new()
        {
            Integer64ID = ID,
        };
        WorkOrderTemplate workOrderTemplate2 = new();

        Assert.False(new WorkOrderTemplateEqualityComparer().Equals(workOrderTemplate1, workOrderTemplate2));
    }

    /// <summary>
    /// The method verifies equality failure when the LastEditedOn property is different between the two objects.
    /// </summary>
    [Fact]
    public void VerifyFailureLastEditedOn()
    {
        WorkOrderTemplate workOrderTemplate1 = new()
        {
            LastEditedOn = DateTime.Now,
        };
        WorkOrderTemplate workOrderTemplate2 = new();

        Assert.False(new WorkOrderTemplateEqualityComparer().Equals(workOrderTemplate1, workOrderTemplate2));
    }

    /// <summary>
    /// The method verifies equality failure when the Name property is different between the two objects.
    /// </summary>
    [Fact]
    public void VerifyFailureName()
    {
        WorkOrderTemplate workOrderTemplate1 = new()
        {
            Name = Name,
        };
        WorkOrderTemplate workOrderTemplate2 = new();

        Assert.False(new WorkOrderTemplateEqualityComparer().Equals(workOrderTemplate1, workOrderTemplate2));
    }

    /// <summary>
    /// The method verifies equality failure when an object and null are compared.
    /// </summary>
    [Fact]
    public void VerifyFailureOneIsNull()
    {
        WorkOrderTemplate workOrderTemplate = new()
        {
            CreatedOn = DateTime.Now,
            DaysDueFromCreation = DaysDueFromCreation,
            Description = Description,
            Integer64ID = ID,
            LastEditedOn = DateTime.Now,
            Name = Name,
            OtherTypeOfService = OtherTypeOfService,
            Priority = WorkOrderPriority.High,
            ServiceType = WorkOrderServiceType.Other,
        };

        Assert.False(new WorkOrderTemplateEqualityComparer().Equals(workOrderTemplate, null));
        Assert.False(new WorkOrderTemplateEqualityComparer().Equals(null, workOrderTemplate));
    }

    /// <summary>
    /// The method verifies equality failure when the OtherTypeOfService property is different between the two objects.
    /// </summary>
    [Fact]
    public void VerifyFailureOtherTypeOfService()
    {
        WorkOrderTemplate workOrderTemplate1 = new()
        {
            OtherTypeOfService = OtherTypeOfService,
        };
        WorkOrderTemplate workOrderTemplate2 = new();

        Assert.False(new WorkOrderTemplateEqualityComparer().Equals(workOrderTemplate1, workOrderTemplate2));
    }

    /// <summary>
    /// The method verifies equality failure when the Priority property is different between the two objects.
    /// </summary>
    [Fact]
    public void VerifyFailurePriority()
    {
        WorkOrderTemplate workOrderTemplate1 = new()
        {
            Priority = WorkOrderPriority.High,
        };
        WorkOrderTemplate workOrderTemplate2 = new();

        Assert.False(new WorkOrderTemplateEqualityComparer().Equals(workOrderTemplate1, workOrderTemplate2));
    }

    /// <summary>
    /// The method verifies equality failure when the ServiceType property is different between the two objects.
    /// </summary>
    [Fact]
    public void VerifyFailureServiceType()
    {
        WorkOrderTemplate workOrderTemplate1 = new()
        {
            ServiceType = WorkOrderServiceType.Other,
        };
        WorkOrderTemplate workOrderTemplate2 = new();

        Assert.False(new WorkOrderTemplateEqualityComparer().Equals(workOrderTemplate1, workOrderTemplate2));
    }

    /// <summary>
    /// The method verifies equality success when two objects (different references) are compared.
    /// </summary>
    [Fact]
    public void VerifySuccess()
    {
        WorkOrderTemplate workOrderTemplate1 = new()
        {
            CreatedOn = DateTime.Now,
            DaysDueFromCreation = DaysDueFromCreation,
            Description = Description,
            Integer64ID = ID,
            LastEditedOn = DateTime.Now,
            Name = Name,
            OtherTypeOfService = OtherTypeOfService,
            Priority = WorkOrderPriority.High,
            ServiceType = WorkOrderServiceType.Other,
        };
        WorkOrderTemplate workOrderTemplate2 = new(workOrderTemplate1);

        Assert.True(new WorkOrderTemplateEqualityComparer().Equals(workOrderTemplate1, workOrderTemplate2));
    }

    /// <summary>
    /// The method verifies equality success when two objects (different references) are compared
    /// but the LastEditedOn property is excluded from the check.
    /// </summary>
    [Fact]
    public void VerifySuccessExcludeCreatedOn()
    {
        WorkOrderTemplate workOrderTemplate1 = new()
        {
            CreatedOn = DateTime.Now,
            DaysDueFromCreation = DaysDueFromCreation,
            Description = Description,
            Integer64ID = ID,
            LastEditedOn = DateTime.Now,
            Name = Name,
            OtherTypeOfService = OtherTypeOfService,
            Priority = WorkOrderPriority.High,
            ServiceType = WorkOrderServiceType.Other,
        };
        WorkOrderTemplate workOrderTemplate2 = new(workOrderTemplate1)
        {
            CreatedOn = DateTime.MinValue,
        };

        Assert.True(new WorkOrderTemplateEqualityComparer(true, false, false).Equals(workOrderTemplate1, workOrderTemplate2));
    }

    /// <summary>
    /// The method verifies equality success when two objects (different references) are compared
    /// but the Integer64ID property is excluded from the check.
    /// </summary>
    [Fact]
    public void VerifySuccessExcludeID()
    {
        WorkOrderTemplate workOrderTemplate1 = new()
        {
            CreatedOn = DateTime.Now,
            DaysDueFromCreation = DaysDueFromCreation,
            Description = Description,
            Integer64ID = ID,
            LastEditedOn = DateTime.Now,
            Name = Name,
            OtherTypeOfService = OtherTypeOfService,
            Priority = WorkOrderPriority.High,
            ServiceType = WorkOrderServiceType.Other,
        };
        WorkOrderTemplate workOrderTemplate2 = new(workOrderTemplate1)
        {
            Integer64ID = ID + 1,
        };

        Assert.True(new WorkOrderTemplateEqualityComparer(false, true, false).Equals(workOrderTemplate1, workOrderTemplate2));
    }

    /// <summary>
    /// The method verifies equality success when two objects (different references) are compared
    /// but the LastEditedOn property is excluded from the check.
    /// </summary>
    [Fact]
    public void VerifySuccessExcludeLastEditedOn()
    {
        WorkOrderTemplate workOrderTemplate1 = new()
        {
            CreatedOn = DateTime.Now,
            DaysDueFromCreation = DaysDueFromCreation,
            Description = Description,
            Integer64ID = ID,
            LastEditedOn = DateTime.Now,
            Name = Name,
            OtherTypeOfService = OtherTypeOfService,
            Priority = WorkOrderPriority.High,
            ServiceType = WorkOrderServiceType.Other,
        };
        WorkOrderTemplate workOrderTemplate2 = new(workOrderTemplate1)
        {
            LastEditedOn = DateTime.MinValue,
        };

        Assert.True(new WorkOrderTemplateEqualityComparer(false, false, true).Equals(workOrderTemplate1, workOrderTemplate2));
    }
}
