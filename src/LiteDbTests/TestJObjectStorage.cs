using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestContracts;

namespace LiteDbTests
{
    [TestClass]
    public class TestJObjectStorage
    {
        [ClassInitialize()]
        public static void ClassInit(TestContext context)
        {
            ConfigurationBuilder configurationBuilder = new ConfigurationBuilder();
            configurationBuilder.AddJsonFile("./config.json");
            Context.Configuration = configurationBuilder.Build();
            ServiceCollection services = new ServiceCollection();

            services.AddSingleton<IConfiguration>(Context.Configuration);
            services.AddTransient<ClientFactory>();
            Context.ServiceProvider = services.BuildServiceProvider();

            TesttSilo.StartSilo().GetAwaiter().GetResult();
        }

        [ClassCleanup()]
        public static void ClassCleanup()
        {
            TesttSilo.StopSilo().GetAwaiter().GetResult();
        }

        [TestMethod]
        public async Task TestStoreRestoreJObject()
        {
            var cl = Context.ServiceProvider.GetService<ClientFactory>();
            await cl.WithClusterClient(async c =>
            {
                try
                {
                    var gr = c.GetGrain<ITestGrain>("test");
                    JObjContract jObj = new JObjContract()
                    {
                        Name = "test",
                        Value = 45
                    };
                    var o = JObject.FromObject(jObj);

                    var d = await gr.SetJOData(o);

                    var res = d.ToObject<JObjContract>();
                    Assert.AreEqual(jObj.Name, res.Name);
                    Assert.AreEqual(jObj.Value, res.Value);
                }
                catch (Exception ex)
                {
                    Assert.Fail(ex.Message);
                }
            });
        }

        [TestMethod]
        public async Task TestTwoProvidersSimultaneous()
        {
            var cl = Context.ServiceProvider.GetService<ClientFactory>();
            await cl.WithClusterClient(async c =>
            {
                try
                {
                    await Task.WhenAll(Task.Run(async () =>
                    {
                        var gr = c.GetGrain<ITestGrain>("test-jobj");
                        JObjContract jObj = new JObjContract()
                        {
                            Name = "test",
                            Value = 45
                        };
                        var o = JObject.FromObject(jObj);

                        var d = await gr.SetJOData(o);

                        var res = d.ToObject<JObjContract>();
                        Assert.AreEqual(jObj.Name, res.Name);
                        Assert.AreEqual(jObj.Value, res.Value);
                    }),
                    Task.Run(async () =>
                    {
                        var gr = c.GetGrain<ITestGrain>("test-std");
                        JObjContract jObj = new JObjContract()
                        {
                            Name = "test2",
                            Value = 46
                        };

                        var res = await gr.SetStdData(jObj);

                        Assert.AreEqual(jObj.Name, res.Name);
                        Assert.AreEqual(jObj.Value, res.Value);
                    }));
                }
                catch (Exception ex)
                {
                    Assert.Fail(ex.Message);
                }
            });
        }
    }
}
