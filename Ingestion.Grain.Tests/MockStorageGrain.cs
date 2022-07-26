using CommunAxiom.Commons.Client.Contracts.Grains.Storage;
using Newtonsoft.Json.Linq;

namespace Ingestion.Grain.Tests
{
    public class MockStorageGrain : IStorageGrain
    {
        public static Dictionary<string, JObject> _data;
        private string _key;

        public MockStorageGrain(string key)
        {
            _data = new Dictionary<string, JObject>();
            _key = key;
        }

        public Task<Dictionary<string, JObject>> GetData()
        {
            return Task.FromResult(_data);
        }

        public Task SaveData(JObject value)
        {
            return Task.Run(() => _data.Add(_key, value));
        }
    }
}