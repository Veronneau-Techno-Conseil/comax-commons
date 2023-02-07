using Orleans;
using System.Threading.Tasks;
using System;
using CommunAxiom.Commons.CommonsShared.Contracts.DataChunk;
using CommunAxiom.Commons.Shared;

namespace CommunAxiom.Commons.CommonsShared.Contracts.DataSeed
{
    public interface IDataSeed: IGrainWithGuidKey
    {
        Task RetrieveData(Guid id); //using DataIndex
        Task SendIndex(DataIndex value); //list of DataIndex
        Task<DataIndex> GetIndex();
        Task<OperationResult> SetUploadStream(Guid id);
        Task Validate(); // ?
    }
}
