using CommunAxiom.Commons.Client.ClusterEventStream.Extentions;
using CommunAxiom.Commons.Shared.RuleEngine;
using Orleans.Streams;

namespace CommunAxiom.Commons.Client.Grains.DispatchGrain.RuleEngine
{
    public class DispatchEventStreamExecutor : IExecutor<Message>
    {
        private readonly IStreamProvider _streamProvider;

        public DispatchEventStreamExecutor(IStreamProvider streamProvider)
        {
            this._streamProvider = streamProvider;
        }

        public async Task Execute(Message param)
        {
            var asyncStream = _streamProvider.GetEventStream();

            await asyncStream.OnNextAsync(param);
        }
    }
}