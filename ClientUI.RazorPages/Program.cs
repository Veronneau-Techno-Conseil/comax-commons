using ClientUI.Shared.Extensions;
using ClientUI.Shared.Models;
using Microsoft.Extensions.Hosting.Internal;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();

#region ConfigureServices

// setting client host environment 
builder.Services.AddSingleton<IHostEnvironment>(
    new HostingEnvironment() { EnvironmentName = builder.Environment.EnvironmentName });

// adding client app settings 
var applicationSettingsSection = builder.Configuration.GetSection("ApplicationSettings");
builder.Services.Configure<ApplicationSettings>(options =>
{
    applicationSettingsSection.Bind(options);
});

// adding application services
builder.Services.SetBlazorApp(applicationSettingsSection.Get<ApplicationSettings>());

#endregion


var app = builder.Build();

app.MapGet("/", () => "Hello World!");

app.Run();
