using JMayer.Example.ASPVanillaMVC;
using JMayer.Example.ASPVanillaMVC.DataLayers;
using JMayer.Example.ASPVanillaMVC.Services;
using JMayer.Example.ASPVanillaMVC.Workers;

//TO DO:
//I need to make sure start date is less than or equal to end date. Built-in Compare attribute only does equal comparison so I need to make something custom. (Will need to add unit tests for this.)
//Unit Tests for the worker. (Not 100% sure how to test this since its dependent on the day its ran and right now, I can't simulate the day its ran.)
//A create work order now button on the schedule page.
//A view only page for created work orders.
//Unit Tests for the view only page.
//Add description to Welcome page.
//Create a readme.

var builder = WebApplication.CreateBuilder(args);

#region Setup Database, Data Layers & Logging

builder.Logging.ClearProviders();
builder.Logging.AddConsole();

WorkOrderTemplateExampleBuilder exampleBuilder = new();
exampleBuilder.Build();

//Add the data layers. Because the example data needs to be built before registration and the data
//layers are memory based, the data layer objects aren't being built with the middleware.
builder.Services.AddSingleton<IWorkOrderDataLayer, WorkOrderDataLayer>();
builder.Services.AddSingleton<IWorkOrderTemplateDataLayer, WorkOrderTemplateDataLayer>(factory => (WorkOrderTemplateDataLayer)exampleBuilder.WorkOrderTemplateDataLayer);
builder.Services.AddSingleton<IWorkOrderTemplateScheduleDataLayer, WorkOrderTemplateScheduleDataLayer>(factory => (WorkOrderTemplateScheduleDataLayer)exampleBuilder.WorkOrderTemplateScheduleDataLayer);

#endregion

#region Setup Services

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddSingleton<IWorkOrderSchedulerService, WorkOrderSchedulerService>();
builder.Services.AddHostedService<WorkOrderSchedulerWorker>();

#endregion

var app = builder.Build();

#region Setup App

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();
app.UseAuthorization();
app.MapStaticAssets();
app.UseStatusCodePagesWithRedirects("~/Home/Error/{0}");

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

#endregion

app.Run();

//Used to expose the launching of the web application to xunit using WebApplicationFactory.
public partial class Program { }
