using System.Reflection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Options;

namespace CommunAxiom.Commons.ClientUI.Shared.JsonLocalizer
{
    public class JsonStringLocalizerFactory : IStringLocalizerFactory
    {
        private string ResourcesPath { get; }

        public JsonStringLocalizerFactory(IOptions<LocalizationOptions> options)
        {
            ResourcesPath = options.Value.ResourcesPath;
        }

        public IStringLocalizer Create(Type resourceSource)
        {
            var assembly = Assembly.GetEntryAssembly();

            var resources = new EmbeddedFileProvider(assembly);
            return new JsonStringLocalizer(resources, ResourcesPath, resourceSource.Name);
        }

        public IStringLocalizer Create(string baseName, string location)
        {
            throw new NotImplementedException();
        }
    }
}

