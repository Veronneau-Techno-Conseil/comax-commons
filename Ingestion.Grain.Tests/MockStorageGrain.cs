using CommunAxiom.Commons.Client.Contracts.Grains.Storage;
using Newtonsoft.Json.Linq;

namespace Ingestion.Grain.Tests
{
    public class MockStorageGrain : IStorageGrain
    {
        public static Dictionary<string, JObject> _data = new Dictionary<string, JObject>();
        private string _key;

        public MockStorageGrain(string key)
        {
            _key = key;
        }

        public Task<JObject> GetData()
        {
            if(_data == null || !_data.ContainsKey(_key))
                return Task.FromResult<JObject>(null);
            return Task.FromResult(_data[_key]);
        }

        public Task SaveData(JObject value)
        {
            return Task.Run(() => _data[_key] = value);
        }
    }
}