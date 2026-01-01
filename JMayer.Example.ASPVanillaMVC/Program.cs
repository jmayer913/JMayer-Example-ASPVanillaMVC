using JMayer.Example.ASPVanillaMVC;
using JMayer.Example.ASPVanillaMVC.DataLayers;

//TO DO:
//I need to make sure start date is less than or equal to end date.
//Unit Tests. (Do for the template & schedule controllers).
//A create work order now button on the schedule page.
//Worker to create work orders based on the schedule. Needs to handle create now requests.
//Unit Tests for the worker.
//A view only page for created work orders.
//Unit Tests for the view only page.

var builder = WebApplication.CreateBuilder(args);

#region Setup Database, Data Layers & Logging

builder.Logging.ClearProviders();
builder.Logging.AddConsole();

WorkOrderTemplateExampleBuilder exampleBuilder = new();
exampleBuilder.Build();

//Add the data layers. Because the example data needs to be built before registration and the data
//layers are memory based, the data layer objects aren't being built with the middleware.
builder.Services.AddSingleton<IWorkOrderTemplateDataLayer, WorkOrderTemplateDataLayer>(factory => (WorkOrderTemplateDataLayer)exampleBuilder.WorkOrderTemplateDataLayer);
builder.Services.AddSingleton<IWorkOrderTemplateScheduleDataLayer, WorkOrderTemplateScheduleDataLayer>(factory => (WorkOrderTemplateScheduleDataLayer)exampleBuilder.WorkOrderTemplateScheduleDataLayer);

#endregion

#region Setup Services

// Add services to the container.
builder.Services.AddControllersWithViews();

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
