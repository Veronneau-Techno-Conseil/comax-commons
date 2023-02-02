using Comax.Commons.Orchestrator.Client;
using Comax.Commons.Orchestrator.Contracts.Portfolio.Models;
using CommunAxiom.Commons.Shared.OIDC;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrchestratorIntegration.Tests.PortfolioRegistry
{
    [TestFixture]
    public class TestPortfolioRegistry
    {
        [Test]
        public async Task GetList()
        {
            var cf = new ClientFactory(Cluster.ServiceProvider, Cluster.Configuration);
            await cf.WithClusterClient(async cc =>
            {
                var reg = cc.GetPortfolioRegistry();
                var lst = await reg.ListPortfolios();
                lst.Should().NotBeNull();
            });
        }

        [Test]
        public async Task TestWrite()
        {
            try
            {
                Cluster.AsCommonsAgent = true;
                var cf = new ClientFactory(Cluster.ServiceProvider, Cluster.Configuration);
                var oidc = new OIDCSettings();
                var oidcOrch = new OIDCSettings();
                Cluster.Configuration.Bind("ClientOIDC", oidc);
                Cluster.Configuration.Bind("OIDC", oidcOrch);
                TokenClient tc = new TokenClient(oidcOrch);
                var (res, auth) = await tc.AuthenticateClient(oidc.ClientId, oidc.Secret, oidc.Scopes);
                var (pRes, principal) = await tc.RequestIntrospection(oidcOrch.ClientId, oidcOrch.Secret, auth.access_token);
                auth.Should().NotBeNull();
                
                var dsid = Guid.NewGuid();
                await cf.WithClusterClient(async cc =>
                {
                    try
                    {
                        var pi = new PortfolioItem
                        {
                            Id= dsid,
                            Description = "description",
                            ItemType = ItemTypes.Datasource,
                            Name = "name",
                            Uri = $"{principal?.GetOwner()}/portfolio/{dsid.ToString()}",
                            PortfolioUri = $"{principal?.GetOwner()}/portfolio"
                        };

                        var reg = cc.GetPortfolioRegistry();
                        var ores = await reg.UpsertPortfolioItem(pi);

                        ores.Should().NotBeNull();
                        ores.IsError.Should().BeFalse();

                        var lst = await reg.ListPortfolios();
                        lst.Should().NotBeNull();
                        lst.Should().NotBeEmpty();
                    }
                    catch(Exception ex)
                    {
                        Assert.Fail(ex.Message);
                    }
                });
            }
            finally
            {
                Cluster.AsCommonsAgent = false;
            }
        }

    }
}
