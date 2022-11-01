using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Comax.Commons.StorageProvider.Serialization
{
    public class StdSerializationConfig
    {
        public bool UseFullAssemblyNames { get; set; }
        public bool IndentJson { get; set; }
        public TypeNameHandling? TypeNameHandling { get; set; }
        public Action<JsonSerializerSettings> ConfigureJsonSerializerSettings { get; set; }
    }
}
