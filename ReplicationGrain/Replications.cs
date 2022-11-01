using System;
using Orleans;
using CommunAxiom.Commons.Client.Contracts.Replication;
using System.Threading.Tasks;

namespace ReplicationGrain
{
    public class Replications : Grain, IReplication
    {
        public Task<string> TestGrain(string Grain)
        {
            return Task.FromResult($"The {Grain} grain has been launched. Check it on the dashboard");
        }
    }
}
