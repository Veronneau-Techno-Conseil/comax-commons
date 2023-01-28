using CommunAxiom.Commons.Client.Grains.DispatchGrain.RuleEngine;
using CommunAxiom.Commons.CommonsShared.Contracts;
using CommunAxiom.Commons.Orleans;
using CommunAxiom.Commons.Shared.RuleEngine;
using Orleans.Streams;
using System;
using System.Collections.Generic;
using System.Text;

namespace DispatchGrain.RuleEngine
{
    public class TargetModifiedEventStreamExecutor : DispatchEventStreamExecutor
    {
        private readonly string _suffix;
        private readonly string? _notifyNamespace;
        private StreamIdResolvingStrategy _resolvingStrategy;
        public TargetModifiedEventStreamExecutor(IStreamProvider streamProvider, IComaxGrainFactory comaxGrainFactory, string suffix, string? notifyNamespace = null, StreamIdResolvingStrategy streamIdResolvingStrategy = StreamIdResolvingStrategy.None) : base(streamProvider, comaxGrainFactory)
        {
            _suffix = suffix;
            _notifyNamespace = notifyNamespace;
            _resolvingStrategy= streamIdResolvingStrategy;
        }

        public override async Task Execute(Message param)
        {
            param.To = $"{param.To.Trim().TrimEnd('/')}/{_suffix}";
            await base.Execute(param);
            await Notify(param);
        }

        private async Task Notify(Message m)
        {
            if (!string.IsNullOrWhiteSpace(_notifyNamespace))
            {
                Guid streamId = Guid.Empty;
                
                switch (_resolvingStrategy)
                {
                    case StreamIdResolvingStrategy.UriId:
                        streamId = await GetUriId(m.To);
                    break;
                    case StreamIdResolvingStrategy.GuidZero:
                        break;
                    default:
                        throw new InvalidOperationException("Resolving strategy is required when notify namespace is set");
                }

                var strm = _streamProvider.GetStream<ClusterNotification>(streamId, _notifyNamespace);
                await strm.OnNextAsync(new ClusterNotification { Verb = ClusterNotification.NEW_MAIL_VERB });
            }
        }

        public enum StreamIdResolvingStrategy
        {
            None,
            GuidZero,
            UriId
        }
    }
}
