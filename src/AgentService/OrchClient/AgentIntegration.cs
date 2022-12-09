using CommunAxiom.Commons.Shared.RuleEngine;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommunAxiom.Commons.Client.AgentService.OrchClient
{
    public static class AgentIntegration
    {
        public static bool IsConnected { get; internal set; }
        private static object _lockObj = new object();
        private static ConcurrentDictionary<Guid, AgentSyncState> workload = new ConcurrentDictionary<Guid, AgentSyncState>();
        public static Guid Register(string token, TimeSpan frequency, TimeSpan subscriptionTimeout)
        {
            var newId = Guid.NewGuid();
            var state = new AgentSyncState(newId, token, frequency, subscriptionTimeout);
            lock (_lockObj)
            {
                var contained = workload.Any(x => x.Value.Token == token);
                if (contained)
                    return workload.First(x => x.Value.Token == token).Key;
                workload.AddOrUpdate(newId, state, (g, x) => state);
            }
            return newId;
        }

        public static bool? IsAuthorized(Guid agentId)
        {
            if (workload.TryGetValue(agentId, out var state))
            {
                return state.IsAuthorized;
            }
            else
            {
                return false;
            }
        }

        public static IEnumerable<Message> Read(Guid guid)
        {
            if(workload.TryGetValue(guid, out var state))
            {
                while(state.Messages.TryDequeue(out var message))
                    yield return message;
            }
        }

        internal static void Remove(Guid syncId)
        {
            workload.TryRemove(syncId, out _);
        }

        public static IEnumerable<AgentSyncState> Jobs
        {
            get
            {
                lock (_lockObj)
                {
                    return workload.Values.ToList();
                }
            }
        }
    }
}
