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
        private readonly IGrainRuntime _grainRuntime;
        public SiloSourcedOutgoingFilter(IGrainRuntime grainRuntime)
        {
            _grainRuntime = grainRuntime;
        }

        public Task Invoke(IOutgoingGrainCallContext context)
        {
            RequestContext.Set("__si", _grainRuntime.SiloIdentity);
            return context.Invoke();
        }
    }
}
