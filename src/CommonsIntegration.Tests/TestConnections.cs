using FluentAssertions;
using NUnit.Framework;
using System;
using CommunAxiom.Commons.Client.AgentClusterRuntime;
using CommunAxiom.Commons.Shared.OIDC;
using Microsoft.Extensions.DependencyInjection;

namespace CommonsIntegration.Tests
{
    [TestFixture]
    public class TestConnections
    {
        [Test]
        public void EmptyTest() 
        {
            Assert.IsTrue(true);
        }

        [Test]
        public async Task GetAccountA()
        {
            var (ret, details) = await Cluster.ClientInstance1.Context.Run(async ctxt =>
            {
                await Cluster.ClientInstance1.WaitForConnection();

                return await ctxt.ClientFactory.WithClusterClient(async cl =>
                {
                    var act = cl.GetAccount();
                    var state = await act.CheckState(true);
                    var details = await act.GetDetails();

                    var pf = cl.GetPortfolio();
                    await pf.AddPortfolio(new CommunAxiom.Commons.Client.Contracts.Grains.Portfolio.PortfolioItem
                    {
                        ID = Guid.NewGuid(),
                        Description = "description",
                        Name = "name",
                        Type = CommunAxiom.Commons.Client.Contracts.Grains.Portfolio.PortfolioType.Dataset
                    });

                    return (state, details);
                });
            }, TimeSpan.FromMinutes(15));
            ret.Should().BeOneOf(CommunAxiom.Commons.Client.Contracts.Account.AccountState.Initial, CommunAxiom.Commons.Client.Contracts.Account.AccountState.ClientMismatch, CommunAxiom.Commons.Client.Contracts.Account.AccountState.Initial);
            details.ClientID.Should().NotBeNull();

            var (commons, user) = await Cluster.ClientInstance1.Context.Run(async ctxt =>
            {
                await Cluster.ClientInstance1.WaitForConnection();
                return await ctxt.ClientFactory.WithClusterClient(async cl =>
                {
                    var act = cl.GetAccount();
                    var state = await act.CheckState(true);
                    var agt = cl.GetAgent();

                    var commonsAuthState = await agt.GetCommonsAuthState();
                    int cnt = 0;
                    while ((!commonsAuthState.IsAuthorised.HasValue || !commonsAuthState.IsAuthorised.HasValue) && cnt < 20)
                    {
                        await Task.Delay(5000);
                        commonsAuthState = await agt.GetCommonsAuthState();
                        cnt++;
                    }

                    cnt = 0;
                    var userAuthState = await agt.GetCurrentUserAuthState();

                    while ((!userAuthState.IsAuthorised.HasValue || !userAuthState.IsAuthorised.HasValue) && cnt < 20)
                    {
                        await Task.Delay(5000);
                        userAuthState = await agt.GetCurrentUserAuthState();
                        cnt++;
                    }

                    return (commonsAuthState, userAuthState);
                });
            }, TimeSpan.FromMinutes(15));
            commons.Should().NotBeNull();
            commons?.IsAuthorised.Should().BeTrue();
            user.Should().NotBeNull();
            user?.IsAuthorised.Should().BeTrue();

            // TODO subscribe mailboax and verify orch ack
            // Mode on to portfolio sync
            var (reference, actual) = await Cluster.CommonsInstance1.Context.Run(async ctxt =>
            {
                var cf = ctxt.Silo.ServiceProvider.SegregatedOrchClientFactory();
                
                var settings = await ctxt.Silo.ServiceProvider.GetService<ISettingsProvider>().GetOIDCSettings();
                TokenClient tokenClient = new TokenClient(settings);
                var (res, data) = await tokenClient.AuthenticateClient(settings.ClientId, settings.Secret, settings.Scopes);

                cf.Token = data.access_token;
                return await cf.WithClusterClient(async cc =>
                {
                    int cnt = 0;
                    var min = DateTime.MinValue.Ticks.ToString();
                    var act = await cc.GetActor($"com://{settings.ClientId}");
                    var text = await act.GetProperty(Comax.Commons.Orchestrator.Contracts.CommonsActor.PropertyTypes.LastPortfolioSync);
                    while((string.IsNullOrWhiteSpace(text) || text == min) && cnt < 20)
                    {
                        await Task.Delay(5000);
                        text = await act.GetProperty(Comax.Commons.Orchestrator.Contracts.CommonsActor.PropertyTypes.LastPortfolioSync);
                        cnt++;
                    }

                    return (min, text);
                });
            }, TimeSpan.FromMinutes(15));

            actual.Should().NotBeEquivalentTo(reference, "LastPortfolioSync should have been updated by the sync mechanism");
        }

        [Test]
        public async Task GetAccountB()
        {
            var (ret, details) = await Cluster.ClientInstance2.Context.Run(async ctxt =>
            {
                await Cluster.ClientInstance2.WaitForConnection();

                return await ctxt.ClientFactory.WithClusterClient(async cl =>
                {
                    var act = cl.GetAccount();
                    var state = await act.CheckState(true);
                    var details = await act.GetDetails();
                    return (state, details);
                });
            });
            ret.Should().BeOneOf(CommunAxiom.Commons.Client.Contracts.Account.AccountState.Initial, CommunAxiom.Commons.Client.Contracts.Account.AccountState.ClientMismatch, CommunAxiom.Commons.Client.Contracts.Account.AccountState.Initial);
            details.ClientID.Should().NotBeNull();
        }
    }
}
