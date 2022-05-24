using System.Globalization;
using System.Reflection;
using System.Text.Json;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Localization;

namespace CommunAxiom.Commons.ClientUI.Shared.JsonLocalizer
{
    public class JsonStringLocalizer : IStringLocalizer
    {
        private IFileProvider FileProvider { get; }
        private string Name { get; }
        private string ResourcesPath { get; }

        public JsonStringLocalizer(IFileProvider fileProvider, string resourcePath, string name)
        {
            FileProvider = fileProvider;
            Name = name;
            ResourcesPath = resourcePath;
        }

        public IEnumerable<LocalizedString> GetAllStrings(bool includeParentCultures)
        {
            throw new NotImplementedException();
        }

        public IStringLocalizer WithCulture(CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        public LocalizedString this[string name]
        {
            get
            {
                var stringMap = LoadStringMap();

                if (stringMap.TryGetValue(name, out string value))
                {
                    return new LocalizedString(name, value);
                }

                return new LocalizedString(name, name);
            }
        }

        public LocalizedString this[string name, params object[] arguments]
        {
            get
            {
                var stringMap = LoadStringMap();

                return new LocalizedString(name, string.Format(stringMap[name], arguments));
            }
        }

        private Dictionary<string, string> LoadStringMap()
        {
            var cultureInfo = CultureInfo.CurrentUICulture;

            var cultureName = cultureInfo.TwoLetterISOLanguageName;
            var path = Path.Combine(ResourcesPath, $"{Name}-{cultureName}.json");

            var fileInfo = FileProvider.GetFileInfo(path);

            if (!fileInfo.Exists)
            {
                return new Dictionary<string, string>();
            }
            using var stream = fileInfo.CreateReadStream();

            return JsonSerializer.DeserializeAsync<Dictionary<string, string>>(stream).Result;
        }
    }
}

