using Orleans;
using Orleans.Runtime;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CommunAxiom.Commons.Orleans.Security
{
    public class SiloSourcedOutgoingFilter : IOutgoingGrainCallFilter
    {
        protected readonly IGrainRuntime _grainRuntime;
        public SiloSourcedOutgoingFilter(IGrainRuntime grainRuntime)
        {
            _grainRuntime = grainRuntime;
        }

        public virtual Task SetSecurityContext()
        {
            RequestContext.Set("__si", _grainRuntime.SiloIdentity);
            return Task.CompletedTask;
        }

        public async Task Invoke(IOutgoingGrainCallContext context)
        {
            await this.SetSecurityContext();
            await context.Invoke();
        }
    }
}
