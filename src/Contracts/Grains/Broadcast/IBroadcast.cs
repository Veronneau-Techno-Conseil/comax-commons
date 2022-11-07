using CommunAxiom.Commons.Shared.RuleEngine;
using Orleans;
using System.Threading.Tasks;

namespace CommunAxiom.Commons.Client.Contracts.Broadcast
{
    public interface IBroadcast : IGrainWithStringKey
    {
        Task Notify(Message message);
    }
}
