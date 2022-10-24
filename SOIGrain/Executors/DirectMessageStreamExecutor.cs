﻿using Comax.Commons.Orchestrator.Contracts.UriRegistry;
using CommunAxiom.Commons.Orleans;
using CommunAxiom.Commons.Shared.RuleEngine;
using Orleans.Streams;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Comax.Commons.Orchestrator.SOIGrain.Executors
{
    public class DirectMessageStreamExecutor: IExecutor<Message>
    {
        private readonly IStreamProvider _streamProvider;
        private readonly IComaxGrainFactory _comaxGrainFactory;
        public DirectMessageStreamExecutor(IStreamProvider streamProvider, IComaxGrainFactory comaxGrainFactory)
        {
            _streamProvider = streamProvider;
            _comaxGrainFactory = comaxGrainFactory;
        }

        public async Task Execute(Message param)
        {
            var grain = _comaxGrainFactory.GetGrain<IUriRegistry>(param.To);
            var g = await grain.GetOrCreate();
            var strm = _streamProvider.GetStream<Message>(g, Constants.DefaultNamespace);
            await strm.OnNextAsync(param);
        }
    }
}
