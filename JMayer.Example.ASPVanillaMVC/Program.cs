using JMayer.Example.ASPVanillaMVC.DataLayers;

var builder = WebApplication.CreateBuilder(args);

#region Setup Database, Data Layers & Logging

builder.Logging.ClearProviders();
builder.Logging.AddConsole();

//Add the data layers. Because the example data needs to be built before registration and the data
//layers are memory based, the data layer objects aren't being built with the middleware.
builder.Services.AddSingleton<IWorkOrderTemplateDataLayer, WorkOrderTemplateDataLayer>();

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

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

#endregion

app.Run();

//Used to expose the launching of the web application to xunit using WebApplicationFactory.
public partial class Program { }
