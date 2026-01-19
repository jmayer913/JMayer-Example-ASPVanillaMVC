using JMayer.Example.ASPVanillaMVC.Models;

namespace TestProject.Test.EqualityComparer;

/// <summary>
/// The class manages testing the work order template schedule equality comparer.
/// </summary>
public class WorkOrderTemplateScheduleUnitTest
{
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
    /// The method verifies equality failure when two nulls are compared.
    /// </summary>
    [Fact]
    public void VerifyFailureBothNull() => Assert.False(new WorkOrderTemplateScheduleEqualityComparer().Equals(null, null));

    /// <summary>
    /// The method verifies equality failure when the CreatedOn property is different between the two objects.
    /// </summary>
    [Fact]
    public void VerifyFailureCreatedOn()
    {
        WorkOrderTemplateSchedule workOrderTemplateSchedule1 = new()
        {
            CreatedOn = DateTime.Now,
        };
        WorkOrderTemplateSchedule workOrderTemplateSchedule2 = new();

        Assert.False(new WorkOrderTemplateScheduleEqualityComparer().Equals(workOrderTemplateSchedule1, workOrderTemplateSchedule2));
    }

    /// <summary>
    /// The method verifies equality failure when the Description property is different between the two objects.
    /// </summary>
    [Fact]
    public void VerifyFailureDescription()
    {
        WorkOrderTemplateSchedule workOrderTemplateSchedule1 = new()
        {
            Description = Description,
        };
        WorkOrderTemplateSchedule workOrderTemplateSchedule2 = new();

        Assert.False(new WorkOrderTemplateScheduleEqualityComparer().Equals(workOrderTemplateSchedule1, workOrderTemplateSchedule2));
    }

    /// <summary>
    /// The method verifies equality failure when the EndDate property is different between the two objects.
    /// </summary>
    [Fact]
    public void VerifyFailureEndDate()
    {
        WorkOrderTemplateSchedule workOrderTemplateSchedule1 = new()
        {
            EndDate = DateTime.Today.AddYears(1),
        };
        WorkOrderTemplateSchedule workOrderTemplateSchedule2 = new();

        Assert.False(new WorkOrderTemplateScheduleEqualityComparer().Equals(workOrderTemplateSchedule1, workOrderTemplateSchedule2));
    }

    /// <summary>
    /// The method verifies equality failure when the ID property is different between the two objects.
    /// </summary>
    [Fact]
    public void VerifyFailureID()
    {
        WorkOrderTemplateSchedule workOrderTemplateSchedule1 = new()
        {
            Integer64ID = ID,
        };
        WorkOrderTemplateSchedule workOrderTemplateSchedule2 = new();

        Assert.False(new WorkOrderTemplateScheduleEqualityComparer().Equals(workOrderTemplateSchedule1, workOrderTemplateSchedule2));
    }

    /// <summary>
    /// The method verifies equality failure when the IsEnabled property is different between the two objects.
    /// </summary>
    [Fact]
    public void VerifyFailureIsEnabled()
    {
        WorkOrderTemplateSchedule workOrderTemplateSchedule1 = new()
        {
            IsEnabled = true,
        };
        WorkOrderTemplateSchedule workOrderTemplateSchedule2 = new();

        Assert.False(new WorkOrderTemplateScheduleEqualityComparer().Equals(workOrderTemplateSchedule1, workOrderTemplateSchedule2));
    }

    /// <summary>
    /// The method verifies equality failure when the LastEditedOn property is different between the two objects.
    /// </summary>
    [Fact]
    public void VerifyFailureLastEditedOn()
    {
        WorkOrderTemplateSchedule workOrderTemplateSchedule1 = new()
        {
            LastEditedOn = DateTime.Now,
        };
        WorkOrderTemplateSchedule workOrderTemplateSchedule2 = new();

        Assert.False(new WorkOrderTemplateScheduleEqualityComparer().Equals(workOrderTemplateSchedule1, workOrderTemplateSchedule2));
    }

    /// <summary>
    /// The method verifies equality failure when the Name property is different between the two objects.
    /// </summary>
    [Fact]
    public void VerifyFailureName()
    {
        WorkOrderTemplateSchedule workOrderTemplateSchedule1 = new()
        {
            Name = Name,
        };
        WorkOrderTemplateSchedule workOrderTemplateSchedule2 = new();

        Assert.False(new WorkOrderTemplateScheduleEqualityComparer().Equals(workOrderTemplateSchedule1, workOrderTemplateSchedule2));
    }

    /// <summary>
    /// The method verifies equality failure when an object and null are compared.
    /// </summary>
    [Fact]
    public void VerifyFailureOneIsNull()
    {
        WorkOrderTemplateSchedule workOrderTemplateSchedule = new()
        {
            CreatedOn = DateTime.Now,
            Description = Description,
            EndDate = DateTime.Today.AddYears(1),
            Integer64ID = ID,
            IsEnabled = true,
            LastEditedOn = DateTime.Now,
            Name = Name,
            ScheduleType = WorkOrderTemplateScheduleType.Quarterly,
            StartDate = DateTime.Today.AddDays(7),
        };

        Assert.False(new WorkOrderTemplateScheduleEqualityComparer().Equals(workOrderTemplateSchedule, null));
        Assert.False(new WorkOrderTemplateScheduleEqualityComparer().Equals(null, workOrderTemplateSchedule));
    }

    /// <summary>
    /// The method verifies equality failure when the ScheduleType property is different between the two objects.
    /// </summary>
    [Fact]
    public void VerifyFailureScheduleType()
    {
        WorkOrderTemplateSchedule workOrderTemplateSchedule1 = new()
        {
            ScheduleType = WorkOrderTemplateScheduleType.Quarterly,
        };
        WorkOrderTemplateSchedule workOrderTemplateSchedule2 = new();

        Assert.False(new WorkOrderTemplateScheduleEqualityComparer().Equals(workOrderTemplateSchedule1, workOrderTemplateSchedule2));
    }

    /// <summary>
    /// The method verifies equality failure when the StartDate property is different between the two objects.
    /// </summary>
    [Fact]
    public void VerifyFailureStartDate()
    {
        WorkOrderTemplateSchedule workOrderTemplateSchedule1 = new()
        {
            StartDate = DateTime.Today.AddDays(7),
        };
        WorkOrderTemplateSchedule workOrderTemplateSchedule2 = new();

        Assert.False(new WorkOrderTemplateScheduleEqualityComparer().Equals(workOrderTemplateSchedule1, workOrderTemplateSchedule2));
    }

    /// <summary>
    /// The method verifies equality success when two objects (different references) are compared.
    /// </summary>
    [Fact]
    public void VerifySuccess()
    {
        WorkOrderTemplateSchedule workOrderTemplateSchedule1 = new()
        {
            CreatedOn = DateTime.Now,
            Description = Description,
            EndDate = DateTime.Today.AddYears(1),
            Integer64ID = ID,
            IsEnabled = true,
            LastEditedOn = DateTime.Now,
            Name = Name,
            ScheduleType = WorkOrderTemplateScheduleType.Quarterly,
            StartDate = DateTime.Today.AddDays(7),
        };
        WorkOrderTemplateSchedule workOrderTemplateSchedule2 = new(workOrderTemplateSchedule1);

        Assert.True(new WorkOrderTemplateScheduleEqualityComparer().Equals(workOrderTemplateSchedule1, workOrderTemplateSchedule2));
    }

    /// <summary>
    /// The method verifies equality success when two objects (different references) are compared
    /// but the LastEditedOn property is excluded from the check.
    /// </summary>
    [Fact]
    public void VerifySuccessExcludeCreatedOn()
    {
        WorkOrderTemplateSchedule workOrderTemplateSchedule1 = new()
        {
            CreatedOn = DateTime.Now,
            Description = Description,
            EndDate = DateTime.Today.AddYears(1),
            Integer64ID = ID,
            IsEnabled = true,
            LastEditedOn = DateTime.Now,
            Name = Name,
            ScheduleType = WorkOrderTemplateScheduleType.Quarterly,
            StartDate = DateTime.Today.AddDays(7),
        };
        WorkOrderTemplateSchedule workOrderTemplateSchedule2 = new(workOrderTemplateSchedule1)
        {
            CreatedOn = DateTime.MinValue,
        };

        Assert.True(new WorkOrderTemplateScheduleEqualityComparer(true, false, false).Equals(workOrderTemplateSchedule1, workOrderTemplateSchedule2));
    }

    /// <summary>
    /// The method verifies equality success when two objects (different references) are compared
    /// but the Integer64ID property is excluded from the check.
    /// </summary>
    [Fact]
    public void VerifySuccessExcludeID()
    {
        WorkOrderTemplateSchedule workOrderTemplateSchedule1 = new()
        {
            CreatedOn = DateTime.Now,
            Description = Description,
            EndDate = DateTime.Today.AddYears(1),
            Integer64ID = ID,
            IsEnabled = true,
            LastEditedOn = DateTime.Now,
            Name = Name,
            ScheduleType = WorkOrderTemplateScheduleType.Quarterly,
            StartDate = DateTime.Today.AddDays(7),
        };
        WorkOrderTemplateSchedule workOrderTemplateSchedule2 = new(workOrderTemplateSchedule1)
        {
            Integer64ID = ID + 1,
        };

        Assert.True(new WorkOrderTemplateScheduleEqualityComparer(false, true, false).Equals(workOrderTemplateSchedule1, workOrderTemplateSchedule2));
    }

    /// <summary>
    /// The method verifies equality success when two objects (different references) are compared
    /// but the LastEditedOn property is excluded from the check.
    /// </summary>
    [Fact]
    public void VerifySuccessExcludeLastEditedOn()
    {
        WorkOrderTemplateSchedule workOrderTemplateSchedule1 = new()
        {
            CreatedOn = DateTime.Now,
            Description = Description,
            EndDate = DateTime.Today.AddYears(1),
            Integer64ID = ID,
            IsEnabled = true,
            LastEditedOn = DateTime.Now,
            Name = Name,
            ScheduleType = WorkOrderTemplateScheduleType.Quarterly,
            StartDate = DateTime.Today.AddDays(7),
        };
        WorkOrderTemplateSchedule workOrderTemplateSchedule2 = new(workOrderTemplateSchedule1)
        {
            LastEditedOn = DateTime.MinValue,
        };

        Assert.True(new WorkOrderTemplateScheduleEqualityComparer(false, false, true).Equals(workOrderTemplateSchedule1, workOrderTemplateSchedule2));
    }
}
