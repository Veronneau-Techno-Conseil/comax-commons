using CommunAxiom.Commons.Client.AgentClusterRuntime.Extentions;
using CommunAxiom.Commons.Shared.RuleEngine;
using Orleans.Streams;
using System.Threading.Tasks;

namespace CommunAxiom.Commons.Client.Grains.BroadcastGrain
{
    public class OrchestratorEventStreamExecutor : IExecutor<Message>
    {
        private readonly IStreamProvider _streamProvider;

        public OrchestratorEventStreamExecutor(IStreamProvider streamProvider)
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
