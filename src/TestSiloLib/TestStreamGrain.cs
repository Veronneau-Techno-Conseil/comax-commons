using CommunAxiom.Commons.Orleans;
using Orleans;
using Orleans.Streams;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace TestSiloLib
{
    public class TestStreamGrain: Grain, TestContracts.ITestStreamGrain
    {
        Guid? _g = null;
        IAsyncStream<string> _stream;
        public Task<Guid> InitStream()
        {
            _g = Guid.NewGuid();
            _stream = this.GetStreamProvider(OrleansConstants.Streams.DefaultStream).GetStream<string>(_g.Value, OrleansConstants.StreamNamespaces.DefaultNamespace);
            return Task.FromResult(_g.Value);
        }

        public async Task PushToStream(string message)
        {
            await _stream.OnNextAsync(message);
        }
    }
}
