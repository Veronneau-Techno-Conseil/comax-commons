using Comax.Commons.Orchestrator.ApiMembershipProvider.ApiRef;

namespace Comax.Commons.Orchestrator.ApiMembershipProvider
{
    public interface ISvcClientFactory
    {
        Task<RefereeSvc> GetRefereeSvc();
    }
}