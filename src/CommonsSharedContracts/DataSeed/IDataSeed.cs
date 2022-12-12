using Orleans;
using System.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Text;

namespace CommunAxiom.Commons.CommonsShared.Contracts.DataSeed
{
    public interface IDataSeed: IGrainWithGuidKey
    {
        Task RetrieveData(Guid id); //using DataIndex
        Task SendIndex(); //list of DataIndex
        Task UploadData(byte[] Data);
        Task Validate(); // ?
    }
}
