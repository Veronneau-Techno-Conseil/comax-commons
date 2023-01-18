using Newtonsoft.Json.Linq;
using Orleans;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CommunAxiom.Commons.CommonsShared.Contracts.DataChunk
{
    public interface IDataChunk : IGrainWithStringKey
    {
        Task SaveData(DataChunkObject value);
        Task<DataChunkObject> GetData();
    }
}
