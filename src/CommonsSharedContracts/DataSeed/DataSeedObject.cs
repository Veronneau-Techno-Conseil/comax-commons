using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text;

namespace CommunAxiom.Commons.CommonsShared.Contracts.DataSeed
{
    public class DataSeedObject
    {       
        public Guid Id { get; set; }
        public JObject Index { get; set; }
        public DataSeedObject() { }
        public DataSeedObject(Guid id, JObject index)
        {
            Id = id;
            Index = index;
        }
    }
}
