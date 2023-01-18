using Orleans;
using System.Threading.Tasks;
using System;
using Comax.Commons.Orchestrator.DataSeedGrain;
using CommunAxiom.Commons.CommonsShared.Contracts.DataChunk;

namespace CommunAxiom.Commons.CommonsShared.Contracts.DataSeed
{
    public interface IDataSeed: IGrainWithGuidKey
    {
        Task RetrieveData(Guid id); //using DataIndex
        Task SendIndex(DataSeedResult value); //list of DataIndex
        Task UploadData(DataChunkResult Data);
        Task Validate(); // ?
    }
}
