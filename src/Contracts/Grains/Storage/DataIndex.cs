using System;
using System.Collections.Generic;
using System.Text;

namespace CommunAxiom.Commons.Client.Contracts.Grains.Storage
{
    public class DataIndex
    {
        public DataIndex()
        {
            Keys = new List<string>();
            Index = new List<DataIndexItem>();  
        }

        public List<string> Keys { get; set; }
        public List<DataIndexItem> Index { get; set; }
    }

    public class DataIndexItem
    {
        public string Id { get; set; }
        public Dictionary<string, string> IndexData { get; set; }
    }
}
