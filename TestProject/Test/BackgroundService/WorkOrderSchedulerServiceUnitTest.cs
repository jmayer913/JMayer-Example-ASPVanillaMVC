using JMayer.Example.ASPVanillaMVC.DataLayers;
using JMayer.Example.ASPVanillaMVC.Models;
using JMayer.Example.ASPVanillaMVC.Services;
using Microsoft.Extensions.Time.Testing;

namespace TestProject.Test.BackgroundService;

/// <summary>
/// The class manages testing the work order scheduler service.
/// </summary>
public class WorkOrderSchedulerServiceUnitTest
{
    /// <summary>
    /// The constant for the number of days in a week.
    /// </summary>
    private const int NumberOfDaysInWeek = 7;

    /// <summary>
    /// The constant for the expected work order count of a daily schedule when ran for the entire year.
    /// </summary>
    private const int ExpectedDailyWorkOrderCount = 365;

    /// <summary>
    /// The constant for the expected work order count of a monthly schedule when ran for the entire year.
    /// </summary>
    private const int ExpectedMonthlyWorkOrderCount = 12;

    /// <summary>
    /// The constant for the expected work order count of a quarterly schedule when ran for the entire year.
    /// </summary>
    private const int ExpectedQuarterlyWorkOrderCount = 4;

    /// <summary>
    /// The constant for the expected work order count of a semiyearly schedule when ran for the entire year.
    /// </summary>
    private const int ExpectedSemiyearlyWorkOrderCount = 2;

    /// <summary>
    /// The constant for the expected work order count of a yearly schedule when ran for the entire year.
    /// </summary>
    private const int ExpectedYearlyWorkOrderCount = 1;

    /// <summary>
    /// The constant for the failure message when the scheduler cannot run when it should.
    /// </summary>
    private const string SchedulerCannotRunFailureMessage = "The scheduler service should have been able to run.";

    /// <summary>
    /// The constant for the failure message when the test failed to setup the template and schedule.
    /// </summary>
    private const string TemplateScheduleSetupFailureMessage = "Failed to create the work order and setup the schedule.";

    /// <summary>
    /// The method creates a work order template and sets up the schedule for the test.
    /// </summary>
    /// <param name="name">The name of the work order template.</param>
    /// <param name="scheduleType">The frequency of the schedule for the template.</param>
    /// <param name="templateDateLayer">Used to create the template.</param>
    /// <param name="scheduleDataLayer">Used to update the schedule.</param>
    /// <returns>True means the work order template was successfully created and the schedule successfully set up.</returns>
    private static async Task<bool> CreateWorkOrderTemplateAsync(string name, WorkOrderTemplateScheduleType scheduleType, bool enabled, WorkOrderTemplateDataLayer templateDateLayer, WorkOrderTemplateScheduleDataLayer scheduleDataLayer)
    {
        _ = await templateDateLayer.CreateAsync(new WorkOrderTemplate()
        {
            Name = name,
        });

        if (await templateDateLayer.GetSingleAsync() is null)
        {
            return false;
        }

        WorkOrderTemplateSchedule? schedule = await scheduleDataLayer.GetSingleAsync();

        if (schedule is null)
        {
            return false;
        }

        schedule.IsEnabled = enabled;
        schedule.ScheduleType = scheduleType;
        schedule.StartDate = new DateTime(DateTime.Today.Year, 1, 1);
        schedule.EndDate = schedule.StartDate.AddYears(1).AddDays(-1);
        _ = await scheduleDataLayer.UpdateAsync(schedule);

        return true;
    }

    /// <summary>
    /// The method verifies the scheduler can create a daily work order for each day.
    /// </summary>
    /// <returns>A task for the async.</returns>
    [Fact]
    public async Task VerifyDailyWorkOrderCreatedOnEachDay()
    {
        FakeTimeProvider fakeTimeProvider = new();
        WorkOrderTemplateDataLayer templateDateLayer = new();
        WorkOrderTemplateScheduleDataLayer scheduleDataLayer = new(templateDateLayer);
        WorkOrderDataLayer workOrderDataLayer = new();
        WorkOrderSchedulerService schedulerService = new(fakeTimeProvider, workOrderDataLayer, templateDateLayer, scheduleDataLayer);

        fakeTimeProvider.SetUtcNow(new DateTimeOffset(DateTime.Today));
        Assert.True(schedulerService.CanCreateWorkOrders(), SchedulerCannotRunFailureMessage);

        bool success = await CreateWorkOrderTemplateAsync("Daily Work Order Created On Each Day Test", WorkOrderTemplateScheduleType.Daily, true, templateDateLayer, scheduleDataLayer);
        Assert.True(success, TemplateScheduleSetupFailureMessage);

        await RunSchedulerForEachDayInYearAsync(fakeTimeProvider, schedulerService);

        long count = await workOrderDataLayer.CountAsync();
        Assert.Equal(ExpectedDailyWorkOrderCount, count);
    }

    /// <summary>
    /// The method verifies the scheduler will ignore a work order template when its schedule is disabled.
    /// </summary>
    /// <returns>A task for the async.</returns>
    [Fact]
    public async Task VerifyDisabledWorkOrderNotCreated()
    {
        FakeTimeProvider fakeTimeProvider = new();
        WorkOrderTemplateDataLayer templateDateLayer = new();
        WorkOrderTemplateScheduleDataLayer scheduleDataLayer = new(templateDateLayer);
        WorkOrderDataLayer workOrderDataLayer = new();
        WorkOrderSchedulerService schedulerService = new(fakeTimeProvider, workOrderDataLayer, templateDateLayer, scheduleDataLayer);

        fakeTimeProvider.SetUtcNow(new DateTimeOffset(DateTime.Today));
        Assert.True(schedulerService.CanCreateWorkOrders(), SchedulerCannotRunFailureMessage);

        bool success = await CreateWorkOrderTemplateAsync("Disabled Work Order Not Created Test", WorkOrderTemplateScheduleType.Daily, false, templateDateLayer, scheduleDataLayer);
        Assert.True(success, TemplateScheduleSetupFailureMessage);

        await schedulerService.CreateWorkOrdersAsync();
        long count = await workOrderDataLayer.CountAsync();
        Assert.Equal(0, count);
    }

    /// <summary>
    /// The method verifies the scheduler can create a monthly work order when the day of execution is the 1st.
    /// </summary>
    /// <returns>A task for the async.</returns>
    [Fact]
    public async Task VerifyMonthlyWorkOrderCreatedOnFirst()
    {
        FakeTimeProvider fakeTimeProvider = new();
        WorkOrderTemplateDataLayer templateDateLayer = new();
        WorkOrderTemplateScheduleDataLayer scheduleDataLayer = new(templateDateLayer);
        WorkOrderDataLayer workOrderDataLayer = new();
        WorkOrderSchedulerService schedulerService = new(fakeTimeProvider, workOrderDataLayer, templateDateLayer, scheduleDataLayer);

        fakeTimeProvider.SetUtcNow(new DateTimeOffset(DateTime.Today));
        Assert.True(schedulerService.CanCreateWorkOrders(), SchedulerCannotRunFailureMessage);

        bool success = await CreateWorkOrderTemplateAsync("Monthly Work Order Created On 1st Test", WorkOrderTemplateScheduleType.Monthly, true, templateDateLayer, scheduleDataLayer);
        Assert.True(success, TemplateScheduleSetupFailureMessage);

        await RunSchedulerForEachDayInYearAsync(fakeTimeProvider, schedulerService);

        long count = await workOrderDataLayer.CountAsync();
        Assert.Equal(ExpectedMonthlyWorkOrderCount, count);
    }

    /// <summary>
    /// The method verifies the scheduler can create a quarterly work order when the day of execution is the 1st for January, April, July and October.
    /// </summary>
    /// <returns>A task for the async.</returns>
    [Fact]
    public async Task VerifyQuarterlyWorkOrderCreatedOnJanuaryAprilJulyOctoberFirst()
    {
        FakeTimeProvider fakeTimeProvider = new();
        WorkOrderTemplateDataLayer templateDateLayer = new();
        WorkOrderTemplateScheduleDataLayer scheduleDataLayer = new(templateDateLayer);
        WorkOrderDataLayer workOrderDataLayer = new();
        WorkOrderSchedulerService schedulerService = new(fakeTimeProvider, workOrderDataLayer, templateDateLayer, scheduleDataLayer);

        fakeTimeProvider.SetUtcNow(new DateTimeOffset(DateTime.Today));
        Assert.True(schedulerService.CanCreateWorkOrders(), SchedulerCannotRunFailureMessage);

        bool success = await CreateWorkOrderTemplateAsync("Quarterly Work Order Created On January 1st, April 1st, July 1st, October 1st Test", WorkOrderTemplateScheduleType.Quarterly, true, templateDateLayer, scheduleDataLayer);
        Assert.True(success, TemplateScheduleSetupFailureMessage);

        await RunSchedulerForEachDayInYearAsync(fakeTimeProvider, schedulerService);

        long count = await workOrderDataLayer.CountAsync();
        Assert.Equal(ExpectedQuarterlyWorkOrderCount, count);
    }

    /// <summary>
    /// The method runs the scheduler service for each day in the current year.
    /// </summary>
    /// <param name="fakeTimeProvider">Used to mock the day the scheduler is ran.</param>
    /// <param name="schedulerService">Used to create work orders for the mocked day.</param>
    /// <returns></returns>
    private static async Task RunSchedulerForEachDayInYearAsync(FakeTimeProvider fakeTimeProvider, WorkOrderSchedulerService schedulerService)
    {
        for (int month = WorkOrderSchedulerService.January; month <= WorkOrderSchedulerService.December; month++)
        {
            int daysInMonth = DateTime.DaysInMonth(DateTime.Today.Year, month);

            for (int day = WorkOrderSchedulerService.FirstOfMonth; day <= daysInMonth; day++)
            {
                DateTime runDay = new(DateTime.Today.Year, month, day);
                fakeTimeProvider.AdjustTime(new DateTimeOffset(runDay));
                await schedulerService.CreateWorkOrdersAsync();
            }
        }
    }

    /// <summary>
    /// The method verifies the scheduler can create a semiyearly work order when the day of execution is the 1st for January and July.
    /// </summary>
    /// <returns>A task for the async.</returns>
    [Fact]
    public async Task VerifySemiyearlyWorkOrderCreatedOnJanuaryJulyFirst()
    {
        FakeTimeProvider fakeTimeProvider = new();
        WorkOrderTemplateDataLayer templateDateLayer = new();
        WorkOrderTemplateScheduleDataLayer scheduleDataLayer = new(templateDateLayer);
        WorkOrderDataLayer workOrderDataLayer = new();
        WorkOrderSchedulerService schedulerService = new(fakeTimeProvider, workOrderDataLayer, templateDateLayer, scheduleDataLayer);

        fakeTimeProvider.SetUtcNow(new DateTimeOffset(DateTime.Today));
        Assert.True(schedulerService.CanCreateWorkOrders(), SchedulerCannotRunFailureMessage);

        bool success = await CreateWorkOrderTemplateAsync("Semiyearly Work Order Created On January 1st, July 1st Test", WorkOrderTemplateScheduleType.Semiyearly, true, templateDateLayer, scheduleDataLayer);
        Assert.True(success, TemplateScheduleSetupFailureMessage);

        await RunSchedulerForEachDayInYearAsync(fakeTimeProvider, schedulerService);

        long count = await workOrderDataLayer.CountAsync();
        Assert.Equal(ExpectedSemiyearlyWorkOrderCount, count);
    }

    /// <summary>
    /// The method verifies the scheduler can create a weekly work order when the day of execution is Monday of this week.
    /// </summary>
    /// <returns>A task for the async.</returns>
    [Fact]
    public async Task VerifyWeeklyWorkOrderCreatedOnMonday()
    {
        FakeTimeProvider fakeTimeProvider = new();
        WorkOrderTemplateDataLayer templateDateLayer = new();
        WorkOrderTemplateScheduleDataLayer scheduleDataLayer = new(templateDateLayer);
        WorkOrderDataLayer workOrderDataLayer = new();
        WorkOrderSchedulerService schedulerService = new(fakeTimeProvider, workOrderDataLayer, templateDateLayer, scheduleDataLayer);

        fakeTimeProvider.SetUtcNow(new DateTimeOffset(DateTime.Today));
        Assert.True(schedulerService.CanCreateWorkOrders(), SchedulerCannotRunFailureMessage);

        bool success = await CreateWorkOrderTemplateAsync("Weekly Work Order Created On Monday Test", WorkOrderTemplateScheduleType.Weekly, true, templateDateLayer, scheduleDataLayer);
        Assert.True(success, TemplateScheduleSetupFailureMessage);

        DateTime startDate = DateTime.Today.AddDays(-(int)DateTime.Today.DayOfWeek);

        for (int day = 0; day < NumberOfDaysInWeek; day++)
        {
            DateTime runDay = startDate.AddDays(day);
            fakeTimeProvider.AdjustTime(new DateTimeOffset(runDay));
            await schedulerService.CreateWorkOrdersAsync();
        }

        long count = await workOrderDataLayer.CountAsync();
        Assert.Equal(1, count);
    }

    /// <summary>
    /// The method verifies the scheduler will ignore a work order template when the schedule has  its schedule has a future start date.
    /// </summary>
    /// <returns>A task for the async.</returns>
    [Fact]
    public async Task VerifyWorkOrderNotCreatedAfterEndDate()
    {
        FakeTimeProvider fakeTimeProvider = new();
        WorkOrderTemplateDataLayer templateDateLayer = new();
        WorkOrderTemplateScheduleDataLayer scheduleDataLayer = new(templateDateLayer);
        WorkOrderDataLayer workOrderDataLayer = new();
        WorkOrderSchedulerService schedulerService = new(fakeTimeProvider, workOrderDataLayer, templateDateLayer, scheduleDataLayer);

        fakeTimeProvider.SetUtcNow(new DateTimeOffset(DateTime.Today));
        Assert.True(schedulerService.CanCreateWorkOrders(), SchedulerCannotRunFailureMessage);

        bool success = await CreateWorkOrderTemplateAsync("Disabled Work Order Not Created Test", WorkOrderTemplateScheduleType.Daily, true, templateDateLayer, scheduleDataLayer);
        Assert.True(success, TemplateScheduleSetupFailureMessage);

        //The default end date used by CreateWorkOrderTemplateAsync() is the last day of this year so have the run day on the day after.
        DateTime runDay = new(DateTime.Today.Year, 1, 1);
        runDay = runDay.AddYears(1);

        fakeTimeProvider.AdjustTime(runDay);
        await schedulerService.CreateWorkOrdersAsync();

        long count = await workOrderDataLayer.CountAsync();
        Assert.Equal(0, count);
    }

    /// <summary>
    /// The method verifies the scheduler will ignore a work order template when its scheduled to start of a future date.
    /// </summary>
    /// <returns>A task for the async.</returns>
    [Fact]
    public async Task VerifyWorkOrderNotCreatedBeforeStartDate()
    {
        FakeTimeProvider fakeTimeProvider = new();
        WorkOrderTemplateDataLayer templateDateLayer = new();
        WorkOrderTemplateScheduleDataLayer scheduleDataLayer = new(templateDateLayer);
        WorkOrderDataLayer workOrderDataLayer = new();
        WorkOrderSchedulerService schedulerService = new(fakeTimeProvider, workOrderDataLayer, templateDateLayer, scheduleDataLayer);

        fakeTimeProvider.SetUtcNow(new DateTimeOffset(DateTime.Today));
        Assert.True(schedulerService.CanCreateWorkOrders(), SchedulerCannotRunFailureMessage);

        bool success = await CreateWorkOrderTemplateAsync("Disabled Work Order Not Created Test", WorkOrderTemplateScheduleType.Daily, true, templateDateLayer, scheduleDataLayer);
        Assert.True(success, TemplateScheduleSetupFailureMessage);

        //The default start date used by CreateWorkOrderTemplateAsync() is the first of this year so have the run day on the day before that.
        DateTime runDay = new(DateTime.Today.Year, 1, 1);
        runDay = runDay.AddDays(-1);

        fakeTimeProvider.AdjustTime(runDay);
        await schedulerService.CreateWorkOrdersAsync();

        long count = await workOrderDataLayer.CountAsync();
        Assert.Equal(0, count);
    }

    /// <summary>
    /// The method verifies the scheduler can create a semiyearly work order when the day of execution is the 1st for January.
    /// </summary>
    /// <returns>A task for the async.</returns>
    [Fact]
    public async Task VerifyYearlyWorkOrderCreatedOnJanuaryFirst()
    {
        FakeTimeProvider fakeTimeProvider = new();
        WorkOrderTemplateDataLayer templateDateLayer = new();
        WorkOrderTemplateScheduleDataLayer scheduleDataLayer = new(templateDateLayer);
        WorkOrderDataLayer workOrderDataLayer = new();
        WorkOrderSchedulerService schedulerService = new(fakeTimeProvider, workOrderDataLayer, templateDateLayer, scheduleDataLayer);

        fakeTimeProvider.SetUtcNow(new DateTimeOffset(DateTime.Today));
        Assert.True(schedulerService.CanCreateWorkOrders(), SchedulerCannotRunFailureMessage);

        bool success = await CreateWorkOrderTemplateAsync("Yearly Work Order Created On January 1st Test", WorkOrderTemplateScheduleType.Yearly, true, templateDateLayer, scheduleDataLayer);
        Assert.True(success, TemplateScheduleSetupFailureMessage);

        await RunSchedulerForEachDayInYearAsync(fakeTimeProvider, schedulerService);

        long count = await workOrderDataLayer.CountAsync();
        Assert.Equal(ExpectedYearlyWorkOrderCount, count);
    }
}
