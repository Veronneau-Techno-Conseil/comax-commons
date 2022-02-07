using Orleans;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CommunAxiom.Commons.Client.Contracts.DataTransfer
{
    public interface IDataTransfer: IGrainWithStringKey   
    {
        Task<string> TestGrain(string Grain);
    }
}
