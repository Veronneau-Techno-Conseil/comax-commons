using Orleans;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CommunAxiom.Commons.Client.Contracts.Dataset
{
    public interface IDataset: IGrainWithStringKey
    {
        Task<string> TestGrain(string Grain);
    }
}
