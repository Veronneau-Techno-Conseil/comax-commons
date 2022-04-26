using CommunAxiom.Commons.Client.Contracts.Dataset;
using Orleans;

namespace DatasetGrain
{
    public class Dataset : Grain, IDataset
    {
        public Task<string> TestGrain(string Grain)
        {
            return Task.FromResult($"The {Grain} grain has been launched. Check it on the dashboard");
        }
    }
}