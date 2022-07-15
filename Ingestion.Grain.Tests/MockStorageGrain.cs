using CommunAxiom.Commons.Client.Contracts.Grains.Storage;
using Newtonsoft.Json.Linq;

namespace Ingestion.Grain.Tests
{
    public class MockStorageGrain : IStorageGrain
    {
        private readonly List<JObject> _data;

        public MockStorageGrain()
        {
            _data = new List<JObject>();
        }

        public Task<List<JObject>> GetData()
        {
            return Task.FromResult(_data);
        }

        public Task SaveData(JObject obj)
        {
            return Task.Run(() => _data.Add(obj));
        }
    }
}