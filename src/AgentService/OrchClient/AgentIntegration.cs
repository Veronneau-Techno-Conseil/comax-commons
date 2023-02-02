using CommunAxiom.Commons.Client.Contracts.Grains.Agent;
using CommunAxiom.Commons.Shared.FlowControl;
using CommunAxiom.Commons.Shared.RuleEngine;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver.Core.Events;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CommunAxiom.Commons.Client.AgentService.OrchClient
{
    public static class AgentIntegration
    {
        public static CancellationToken CancellationToken { get; set; }

        private static object _lockObj = new object();
        private static ConcurrentDictionary<Guid, ServiceHost> workload = new ConcurrentDictionary<Guid, ServiceHost>();
        private static NoFlowTask<object> noFlow = new NoFlowTask<object>(() => Task.FromResult(new object()));
        public static async Task Register(Guid id, string token, IConfiguration configuration)
        {
            await noFlow.Run((o) =>
            {
                workload.AddOrUpdate(id,
                    i =>
                    {
                        var svc = new ServiceHost(i, configuration);
                        svc.Token = token;
                        _ = svc.StartAsync(CancellationToken);
                        return svc;
                    },
                    (i, h) =>
                    {
                        h.Token = token;
                        return h;
                    });
                return Task.FromResult(true);
            });
        }

        public static IAgentSyncStatus? GetStatus(Guid agentId)
        {
            if (workload.TryGetValue(agentId, out var state))
            {
                return state;
            }
            return null;
        }

        public static async Task Clear()
        {
            foreach (var it in workload)
            {
                await it.Value.StopAsync();
                it.Value.Dispose();
            }
            workload.Clear();
        }

        internal static void Remove(Guid syncId)
        {
            if (workload.TryRemove(syncId, out ServiceHost value))
            {
                value.Dispose();
            };

        }
    }
}
