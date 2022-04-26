using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Orleans;
using Orleans.Runtime;
using Orleans.Serialization;
using System;
using System.Collections.Generic;
using System.Text;

namespace Comax.Commons.StorageProvider.Serialization
{
    internal class NewtonsoftSerializationProvider : ISerializationProvider
    {
        private readonly IConfiguration configuration;
        private JsonSerializerSettings _jsonSettings;
        private StdSerializationConfig _config;
        private readonly ITypeResolver _typeResolver;
        private readonly IGrainFactory _grainFactory;
        public NewtonsoftSerializationProvider(IConfiguration configuration, ITypeResolver typeResolver, IGrainFactory grainFactory)
        {
            this.configuration = configuration;

            _typeResolver = typeResolver;
            _grainFactory = grainFactory;
        }

        public void Configure(string name)
        {
            _config = new StdSerializationConfig();
            configuration.GetSection(name).Bind(_config);
            _jsonSettings = OrleansJsonSerializer.UpdateSerializerSettings(
                    OrleansJsonSerializer.GetDefaultSerializerSettings(_typeResolver, _grainFactory),
                    _config.UseFullAssemblyNames, 
                    _config.IndentJson, 
                    _config.TypeNameHandling);
        }

        public object Deserialize(byte[] obj)
        {
            var str = Encoding.UTF8.GetString(obj);
            var data = JsonConvert.DeserializeObject<object>(str, _jsonSettings);
            return data;
        }

        public byte[] Serialize(object obj)
        {
            var data = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(obj, _jsonSettings));
            return data;
        }
    }
}
