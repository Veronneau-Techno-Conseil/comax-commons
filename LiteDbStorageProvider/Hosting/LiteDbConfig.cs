using System;
using System.Collections.Generic;
using System.Text;

namespace Comax.Commons.StorageProvider.Hosting
{
    public class LiteDbConfig
    {
        public string Name { get; set; } = "LiteDbStorage";
        public string FileName { get; set; }
        public string Password { get; set; }
        public string SerializationProvider { get; set; } = "standard";
        public string SerializationConfig { get; set; } 
    }
}
