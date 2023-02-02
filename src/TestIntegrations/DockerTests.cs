using FluentAssertions;
using NUnit.Framework;
using System;
using System.Threading.Tasks;

namespace TestIntegrations
{
    [TestFixture]
    public class DockerTests
    {
        [Test]
        public async Task TestConnect()
        {
            DockerIntegration.Client client= new DockerIntegration.Client();
            var res = await client.InstallContainer("formio", "nginx", new System.Collections.Generic.List<(int, int)> { (80, 8080) });

            res.Should().NotBeNullOrEmpty();
        }
    }
}
