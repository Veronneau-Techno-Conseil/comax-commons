using System;
using System.Collections.Generic;
using System.Text;

namespace Comax.Commons.StorageProvider.Hosting
{
    public class MongodbConfig
    {
        public string ConnectionString { get; set; }
        public string DatabaseName { get; set; }
        public string Collection { get; set; }
        public string SerializationProvider { get; set; }
        public string SerializationConfig { get; set; }
    }
}
