using CommunAxiom.Commons.Client.Contracts;
using Newtonsoft.Json.Linq;
using Orleans.Runtime;
using System.Threading.Tasks;

namespace TestContracts
{
    public class TestGrain : Orleans.Grain, ITestGrain
    {
        private readonly IPersistentState<JObject> _jobjDetails;
        private readonly IPersistentState<JObjContract> _stdDetails;
        public TestGrain([PersistentState("test-jobj-state", Constants.Storage.JObjectStore)] IPersistentState<JObject> jobjDetails,
            [PersistentState("test-std-state")] IPersistentState<JObjContract> stdDetails)
        {
            _jobjDetails = jobjDetails;
            _stdDetails = stdDetails;
        }
        public async Task<JObject> SetJOData(JObject data)
        {
            _jobjDetails.State = data;
            await _jobjDetails.WriteStateAsync();
            await _jobjDetails.ReadStateAsync();
            return _jobjDetails.State;
        }

        public async Task<JObjContract> SetStdData(JObjContract data)
        {
            _stdDetails.State = data;
            await _stdDetails.WriteStateAsync();
            await _stdDetails.ReadStateAsync();
            return _stdDetails.State;
        }
    }
}
