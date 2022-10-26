using Newtonsoft.Json.Linq;
using Orleans;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CommunAxiom.Commons.Client.Contracts.Grains.Storage
{
    public interface IStorage: IGrainWithStringKey
    {
        Task SaveData(JObject o);
    }
}
