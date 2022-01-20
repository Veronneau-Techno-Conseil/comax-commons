using Orleans;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CommunAxiom.Commons.Client.Contracts.Replication
{
    public interface IReplication: IGrainWithIntegerKey
    {
        Task<string> TestGrain(string Grain);
    }
}
