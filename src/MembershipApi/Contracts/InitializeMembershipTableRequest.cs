using Microsoft.JSInterop;

namespace CommunAxiom.Commons.CommonsShared.MembershipApi.Contracts
{
    public class InitializeMembershipTableRequest
    {
        public bool TryInitTableVersion { get; set; }
        public string ClusterId { get; set; }

    }
}