using System;
using CommunAxiom.Commons.Client.Contracts;
using CommunAxiom.Commons.Orleans;
using CommunAxiom.Commons.Shared.RuleEngine;
using Orleans.Streams;

namespace CommunAxiom.Commons.Client.AgentClusterRuntime.Extentions
{
    public static class EventStreamExtentions
    {
        public static IAsyncStream<Message> GetEventStream(this IStreamProvider streamProvider)
        {
            return streamProvider.GetStream<Message>(Guid.Empty, OrleansConstants.StreamNamespaces.DefaultNamespace);
        } 
    }
}
