using Blazorise;
using Blazorise.Bulma;
using Blazorise.Icons.FontAwesome;
using CommunAxiom.Commons.Client.ClusterClient;
using CommunAxiom.Commons.ClientUI.Server.Helper;
using CommunAxiom.Commons.ClientUI.Server.Models;
using CommunAxiom.Commons.ClientUI.Server.SEO;
using CommunAxiom.Commons.ClientUI.Shared.Extensions;
using CommunAxiom.Commons.ClientUI.Shared.Models;
using CommunAxiom.Commons.ClientUI.Shared.Services;
using CommunAxiom.Commons.Orleans.Security;
using ElectronNET.API;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting.Internal;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Microsoft.AspNetCore.ResponseCompression;
using System.Text;
using CommunAxiom.Commons.ClientUI.Server.Hubs;
using CommunAxiom.Commons.Ingestion.Extentions;
using CommunAxiom.Commons.Shared.OIDC;
using Orleans;
//  TODO: webBuilder.UseElectron(args);

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSignalR();
builder.Services.AddResponseCompression(opts =>
{
    opts.MimeTypes = ResponseCompressionDefaults.MimeTypes.Concat(
        new[] { "application/octet-stream" });
});

builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();

builder.Services.AddBlazorise(options => { options.Immediate = true; });

builder.Services.AddBulmaProviders();
builder.Services.AddFontAwesomeIcons();
builder.Services.AddIngestion();
builder.Services.AddCors(option =>
{
    option.AddDefaultPolicy(cp => { cp.SetIsOriginAllowed(origin => new Uri(origin).IsLoopback); });
});

#region ConfigureServices

builder.Services.AddHttpContextAccessor();

// setting client host environment 
builder.Services.AddSingleton<IHostEnvironment>(
    new HostingEnvironment { EnvironmentName = builder.Environment.EnvironmentName });

// adding client app settings 
var applicationSettingsSection = builder.Configuration;
builder.Services.Configure<ApplicationSettings>(options => { applicationSettingsSection.Bind(options); });

// SEO Services
builder.Services.AddScoped<MetadataTransferService>();
builder.Services.AddMemoryCache();
builder.Services.AddSingleton<CommunAxiom.Commons.ClientUI.Server.Helper.ITempData, CommunAxiom.Commons.ClientUI.Server.Helper.TempStorage>();

// Orleans client
builder.Services.AddLogging(x => x.AddConsole());
builder.Services.AddTransient<ITokenProvider, ClientTokenProvider>();
builder.Services.AddSingleton<IOutgoingGrainCallFilter, SecureTokenOutgoingFilter>();
builder.Services.SetupOrleansClient("./appsettings.json");

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

app.UseResponseCompression();

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
    endpoints.MapHub<SystemHub>("/systemhub");
});
app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

app.Run();