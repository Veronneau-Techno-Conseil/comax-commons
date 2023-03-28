
using CommunAxiom.DotnetSdk.Helpers.OIDC;
using GrainStorageService;
using GrainStorageService.Models.Configurations;
using Microsoft.Extensions.Options;

var app = await Application.CreateApp("./config.json");
app.Run();
