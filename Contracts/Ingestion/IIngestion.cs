using Orleans;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CommunAxiom.Commons.Client.Contracts.Ingestion
{
    public interface IIngestion: IGrainWithIntegerKey
    {
        Task<string> TestGrain(string Grain);
    }
}
