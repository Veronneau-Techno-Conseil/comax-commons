using CommunAxiom.Commons.Client.Contracts.DataTransfer;
using Orleans;
using System;
using System.Threading.Tasks;

namespace DataTransferGrain
{
    public class DataTransfer : Grain, IDataTransfer
    {
        public Task<string> TestGrain(string Grain)
        {
            return Task.FromResult($"The {Grain} grain has been launched. Check it on the dashboard");
        }
    }
}
