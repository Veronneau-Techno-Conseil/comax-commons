using Orleans;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Comax.Commons.Orchestrator.Contracts.Central
{
    public interface ICentral: IGrainWithGuidKey 
    {
        Task<IEnumerable<Contact>?> GetContacts();
        Task<bool> CanMessage(string userId);
    }
}
