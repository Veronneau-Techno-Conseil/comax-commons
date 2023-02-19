using System;
using System.Collections.Generic;
using System.Text;

namespace Comax.Commons.StorageProvider
{
    public class ApiStorageConfiguration
    {
        public string ApiStorageUri { get; set; }
        public string SerializationProvider { get; set; } = "standard";
        public string SerializationConfig { get; set; }
    }
}
