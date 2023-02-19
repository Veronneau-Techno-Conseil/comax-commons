using FluentAssertions;
using Newtonsoft.Json;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace TestIntegrations
{
    [TestFixture]
    public class DockerTests
    {
        [Test]
        public async Task TestDico1()
        {
            Dictionary<string, object> test = new Dictionary<string, object>();

            test.Add("foo", "bar");
            test.Add("foo12", new Dictionary<string, object>()
            {
                {"fo3o", "barwef"},
            });

            string str = JsonConvert.SerializeObject(test);

            str = System.Text.Json.JsonSerializer.Serialize(test);
        }

        [Test]
        public async Task TestConnect()
        {
            try
            {
                DockerIntegration.Client client = new DockerIntegration.Client();
                var res = await client.InstallContainer("mynginx", "nginx", "latest", new Dictionary<string, string> { { "80/tcp", "8080" } }, new System.Collections.Generic.List<string>() { "blah=blu" });

                res.Should().BeTrue();
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}
