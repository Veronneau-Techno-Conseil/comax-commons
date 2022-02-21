using CommunAxiom.Commons.Client.Contracts.Ingestion;
using Orleans;
using System;
using System.Threading.Tasks;

namespace IngestionGrain
{
    public class Ingestions : Grain, IIngestion
    {
        public Task<string> TestGrain(string Grain)
        {
            return Task.FromResult($"The {Grain} grain has been launched. Check it on the dashboard");
        }
    }
}
