using CommunAxiom.Commons.Client.AgentClusterRuntime.Extentions;
using CommunAxiom.Commons.CommonsShared.Contracts.EventMailbox;
using CommunAxiom.Commons.CommonsShared.Contracts.UriRegistry;
using CommunAxiom.Commons.Orleans;
using CommunAxiom.Commons.Shared.RuleEngine;
using Orleans.Streams;

namespace CommunAxiom.Commons.Client.Grains.DispatchGrain.RuleEngine
{
    public class DispatchEventStreamExecutor : IExecutor<Message>
    {
        protected readonly IStreamProvider _streamProvider;
        protected readonly IComaxGrainFactory _comaxGrainFactory;
        public DispatchEventStreamExecutor(IStreamProvider streamProvider, IComaxGrainFactory comaxGrainFactory)
        {
            this._streamProvider = streamProvider;
            this._comaxGrainFactory = comaxGrainFactory;
        }

        public virtual async Task Execute(Message param)
        {
            var uid = await GetUriId(param.To);
            var asyncStream = _streamProvider.GetStream<Message>(uid, OrleansConstants.StreamNamespaces.MAILBOX_STREAM_INBOUND_NS);
            
            await asyncStream.OnNextAsync(param);
        }

        protected async Task<Guid> GetUriId(string uri)
        {
            var uriObj = _comaxGrainFactory.GetGrain<IUriRegistry>(uri);
            var uid = await uriObj.GetOrCreate();
            return uid;
        }
    }
}