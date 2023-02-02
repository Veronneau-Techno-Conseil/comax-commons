using Comax.Commons.Orchestrator.MembershipProvider;
using CommunAxiom.CentralApi;
using MongoDB.Driver;
using Orleans;
using System.Security.Cryptography.X509Certificates;
using OpenIddict.Validation.AspNetCore;
using Orleans.Configuration;
using Comax.Commons.Orchestrator.MembershipProvider.Models;
using Comax.Commons.Orchestrator.MongoDbMembershipStorage;
using AgentReferee;

var app = new RefereeApp();
app.Init();
app.Run();
