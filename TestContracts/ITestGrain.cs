using Newtonsoft.Json.Linq;
using Orleans;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestContracts
{
    public interface ITestGrain : IGrainWithStringKey
    {
        Task<JObject> SetJOData(JObject data);
        Task<JObjContract> SetStdData(JObjContract data);
    }
}
