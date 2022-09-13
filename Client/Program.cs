using Blazorise;
using Blazorise.Bulma;
using Blazorise.Icons.FontAwesome;
using ClusterClient;
using CommunAxiom.Commons.ClientUI.Server.Models;
using CommunAxiom.Commons.ClientUI.Server.SEO;
using CommunAxiom.Commons.ClientUI.Shared.Extensions;
using CommunAxiom.Commons.ClientUI.Shared.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting.Internal;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;

//namespace CommunAxiom.Commons.ClientUI;

//public class Program
//{
//    public static void Main(string[] args)
//    {
//        CreateHostBuilder(args).Build().Run();
//    }

//    public static IHostBuilder CreateHostBuilder(string[] args) =>
//        Host.CreateDefaultBuilder(args)
//            .ConfigureWebHostDefaults(webBuilder =>
//            {
//                webBuilder.UseStartup<Startup>();
//                webBuilder.UseElectron(args);
//            });
//}

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();

builder.Services.AddBlazorise(options =>
{
    options.Immediate = true;
});

builder.Services.AddBulmaProviders();
builder.Services.AddFontAwesomeIcons();

builder.Services.AddCors(option =>
{
    option.AddDefaultPolicy(cp =>
    {
        cp.SetIsOriginAllowed(origin => new Uri(origin).IsLoopback);
    });
});

#region ConfigureServices


// setting client host environment 
builder.Services.AddSingleton<IHostEnvironment>(
    new HostingEnvironment() { EnvironmentName = builder.Environment.EnvironmentName });

// adding client app settings 
var applicationSettingsSection = builder.Configuration;
builder.Services.Configure<ApplicationSettings>(options =>
{
    applicationSettingsSection.Bind(options);
});

// SEO Services
builder.Services.AddScoped<MetadataTransferService>();
builder.Services.AddMemoryCache();
builder.Services.AddSingleton<CommunAxiom.Commons.ClientUI.Server.Helper.ITempData, CommunAxiom.Commons.ClientUI.Server.Helper.TempStorage>();
builder.Services.SetupOrleansClient();

builder.Services.AddControllers();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "BlazingChat.WebAPI", Version = "v1" });
});
builder.Services.AddSignalR();
builder.Services.AddDbContextFactory<LoggingContext>(options => options.UseSqlite("Name=LoggingDb"));

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = applicationSettingsSection["Jwt:Issuer"],
        ValidAudience = applicationSettingsSection["Jwt:Issuer"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(applicationSettingsSection["Jwt:Key"]))
    };
});

// adding application services
builder.Services.SetBlazorApp(applicationSettingsSection.Get<ApplicationSettings>());


#endregion

var app = builder.Build();


app.UseRequestLocalization();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}



app.UseSwagger();
app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "BlazingChat.WebAPI v1"));

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseCors();

app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.UseEndpoints(endpoints =>
{
    endpoints.MapControllerRoute(
        name: "default",
        pattern: "{controller}/{action=Index}/{id?}");
});
app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

app.Run();