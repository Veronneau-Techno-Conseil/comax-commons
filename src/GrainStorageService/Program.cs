using CommunAxiom.Commons.Shared.OIDC;
using GrainStorageService;
using GrainStorageService.Models.Configurations;
using Microsoft.Extensions.Options;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
// Add services to the container.
builder.Services.AddRazorPages();
// Add services to the container.
builder.Services.Configure<GrainStorageService.DbConf>(x => builder.Configuration.GetSection("DbConfig").Bind(x));
builder.Services.AddDbContext<GrainStorageService.Models.StorageDbContext>();

Console.WriteLine($"Setting services...");
builder.Services.Configure<OIDCSettings>(x => builder.Configuration.GetSection("OIDC").Bind(x));


var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();
// Configure the HTTP request pipeline.
//if (!app.Environment.IsDevelopment())
//{
app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();

#if DEBUG
Console.WriteLine("Launching mariadb in debug");
    var dbConf = app.Services.GetService<IOptionsMonitor<GrainStorageService.DbConf>>();
    var curconf = dbConf.CurrentValue;
    try
    {
        DockerIntegration.Client client = new DockerIntegration.Client();
        await client.InstallContainer("mariadbtest", "mariadb", "10.10",
            new Dictionary<string, string> { { "3306/tcp", curconf.Port } },
            new List<string> { $"MYSQL_ROOT_PASSWORD={curconf.Password}" });
    } catch(Exception ex)
    {
        throw;
    }

#endif
//}
Console.WriteLine($"Migrate db...");
app.Services.MigrateDb();

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.MapRazorPages();
app.Run();
