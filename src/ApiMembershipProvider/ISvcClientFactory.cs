using Comax.Commons.CommonsShared.ApiMembershipProvider.ApiRef;

namespace Comax.Commons.CommonsShared.ApiMembershipProvider
{
    public interface ISvcClientFactory
    {
        Task<RefereeSvc> GetRefereeSvc();
    }
}