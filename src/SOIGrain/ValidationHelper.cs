using Comax.Commons.Orchestrator.Contracts.Central;
using CommunAxiom.Commons.Orleans;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Comax.Commons.Orchestrator.SOIGrain
{
    public static class ValidationHelper
    {
        public static async Task<bool> AuthorizeDirectUserMessage(IComaxGrainFactory comaxGrainFactory, string targetUser)
        {
            var central = comaxGrainFactory.GetGrain<ICentral>(Guid.Empty);
            return await central.CanMessage(targetUser);
        }

        internal static Task<bool> ValidateOrchId(IComaxGrainFactory comaxGrainFactory, string v)
        {
            //TODO: Complete Central api to validate app identification
            return Task.FromResult(true);
        }
    }
}
