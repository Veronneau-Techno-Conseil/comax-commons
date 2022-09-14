﻿using System;
using CommunAxiom.Commons.Client.Contracts;
using CommunAxiom.Commons.Orleans;
using CommunAxiom.Commons.Shared.RuleEngine;
using Orleans.Streams;

namespace CommunAxiom.Commons.Client.ClusterEventStream.Extentions
{
    public static class EventStreamExtentions
    {
        public static IAsyncStream<Message> GetEventStream(this IStreamProvider streamProvider)
        {
            return streamProvider.GetStream<Message>(Guid.Empty, Constants.DefaultNamespace);
        } 
    }
}