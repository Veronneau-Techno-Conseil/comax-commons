using Comax.Commons.Orchestrator.MembershipProvider;
using CommunAxiom.CentralApi;
using MongoDB.Driver;
using Orleans;
using System.Security.Cryptography.X509Certificates;
using OpenIddict.Validation.AspNetCore;
using Orleans.Configuration;

var builder = WebApplication.CreateBuilder(args);

builder.Logging.AddConsole();

Console.WriteLine($"Using kestrel on {builder.Configuration["Urls"]}...");
builder.WebHost.UseKestrel(opts =>
{
    if (builder.Configuration["Urls"].StartsWith("https"))
    {
        opts.ConfigureHttpsDefaults(def =>
        {
            var certPem = File.ReadAllText("cert.pem");
            var eccPem = File.ReadAllText("key.pem");

            var cert = X509Certificate2.CreateFromPem(certPem, eccPem);
            def.ServerCertificate = new X509Certificate2(cert.Export(X509ContentType.Pkcs12));
        });
    }
});

builder.Services.Configure<OidcConfig>(x => builder.Configuration.GetSection("OIDC").Bind(x));


// Add services to the container.

builder.Services.AddControllers()
    .AddJsonOptions(x=> {
        x.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
    });
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle

builder.Services.AddSingleton<IMongoClientFactory>(sp =>
{
    return new MongoClientFactory(builder.Configuration);
});

builder.Services.Configure<MongoDBOptions>(mo =>
{
    mo.DatabaseName = "clustermembers";
    mo.ClientName = "member_mongo";
    mo.CollectionConfigurator = cs =>
    {
        cs.WriteConcern = WriteConcern.Acknowledged;
        cs.ReadConcern = ReadConcern.Local;
    };

});

builder.Services.Configure<ClusterOptions>(options =>
{
    options.ClusterId = "0.0.1-a1";
    options.ServiceId = "OrchestratorCluster";
});

builder.Services.AddSingleton<IMembershipTable, MongoMembershipTable>();

// Register the OpenIddict validation components.
builder.Services.AddOpenIddict()
    .AddValidation(options =>
    {
        OidcConfig oidcConfig = new OidcConfig();
        builder.Configuration.GetSection("OIDC").Bind(oidcConfig);
        // Note: the validation handler uses OpenID Connect discovery
        // to retrieve the address of the introspection endpoint.
        options.SetIssuer(oidcConfig.Authority);
        //options.AddAudiences("contacts_oi");

        // Configure the validation handler to use introspection and register the client
        // credentials used when communicating with the remote introspection endpoint.

        options.UseIntrospection()
               .SetClientId(oidcConfig.ClientId)
               .SetClientSecret(oidcConfig.Secret);

        // Register the System.Net.Http integration.
        options.UseSystemNetHttp();

        // Register the ASP.NET Core host.
        options.UseAspNetCore();
    });

builder.Services.AddAuthentication(OpenIddictValidationAspNetCoreDefaults.AuthenticationScheme);

builder.Services.AddAuthorization(opt =>
{
    opt.AddPolicy("Authenticated", act =>
    {
        act.RequireAssertion(ctxt =>
        {
            return ctxt.User != null;
        });
    });

    opt.AddPolicy("Reader", builder =>
    {
        builder.RequireClaim("https://referee.communaxiom.org/reader", "access");
    });

    opt.AddPolicy("Actor", builder =>
    {
        builder.RequireAssertion(ctxt =>
        {
            //var con = (HttpContext)ctxt.Resource;
            //con.Request.Body.Position = 0;
            //StreamReader rdr = new StreamReader(con.Request.Body);
            //var res = rdr.ReadToEndAsync().GetAwaiter().GetResult();
            return ctxt.User.HasClaim(x => x.Type == "https://referee.communaxiom.org/actor");
        });
        //builder.RequireClaim("https://referee.communaxiom.org/actor", "access");
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseSwagger();
app.UseSwaggerUI();

if (builder.Configuration["Urls"].StartsWith("https"))
{
    app.UseHttpsRedirection();
}

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
