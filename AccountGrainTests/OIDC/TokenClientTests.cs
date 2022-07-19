using CommunAxiom.Commons.Client.Grains.AccountGrain;
using CommunAxiom.Commons.Shared;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountGrainTests.OIDC
{
    [TestClass]
    public class TokenClientTests
    {
        [TestMethod]
        public async Task InitializeOIDC()
        {
            TokenClient tokenClient = new TokenClient(Context.Configuration);
            await tokenClient.Configure();
            Assert.IsNotNull(tokenClient.TokenMetadata);
            Assert.IsNotNull(tokenClient.TokenMetadata.TokenEndpoint);
        }

        [TestMethod]
        [Ignore]
        public async Task Authenticate()
        {
            //TODO: create test account to support test clientid / secrets
            TokenClient tokenClient = new TokenClient(Context.Configuration);
            var (success, token) = await tokenClient.GetToken("convicia_magnosque_imperiis_monstruosi", "6b99f7a4-cee1-43a6-a960-e22db4df121a", "openid offline_access");
            Assert.IsTrue(success);
            Assert.IsNotNull(token);
            Assert.IsNotNull(token.access_token);
            Assert.IsNotNull(token.refresh_token);
        }
    }
}
