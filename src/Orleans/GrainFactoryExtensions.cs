using CommunAxiom.Commons.Orleans;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using gf = Orleans.IGrainFactory;
using gr = Orleans.Grain;
namespace CommunAxiom.Commons.Orleans
{
    public static class GrainFactoryExtensions
    {
        public static IComaxGrainFactory AsComax(this gf gf, GetStreamProvider getStreamProvider)
        {
            return new GrainFactory(gf, getStreamProvider);
        }
    }
}
