using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AccountGrainTests
{
    [TestClass()]
    public static class Setup
    {
        [AssemblyInitialize()]
        public static void AssemblyInit(TestContext context)
        {
            ConfigurationBuilder configurationBuilder = new ConfigurationBuilder();
            configurationBuilder.AddJsonFile("./config.json");
            Context.Configuration = configurationBuilder.Build();
        }
    }
}