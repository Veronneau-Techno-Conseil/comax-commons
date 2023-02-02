using Comax.Commons.Orchestrator.Contracts.Central;
using Comax.Commons.Orchestrator.Contracts.CommonsActor;
using Comax.Commons.Orchestrator.Contracts.PublicBoard;
using Comax.Commons.Orchestrator.Contracts.SOI;
using CommunAxiom.Commons.CommonsShared.Contracts.EventMailbox;
using CommunAxiom.Commons.CommonsShared.Contracts.UriRegistry;
using CommunAxiom.Commons.Orleans;
using CommunAxiom.Commons.Orleans.Security;
using System;
using System.Threading.Tasks;

namespace CommunAxiom.Commons.Orleans
{
    public static class Client 
    {
        public static async Task<UserTuple> LoadUser(this IComaxGrainFactory grainFactory)
        {
            var _currentUser = await grainFactory.GetUriRegistry("").GetCurrentUser();
            return _currentUser;
        }

        public static IUriRegistry GetUriRegistry(this IComaxGrainFactory grainFactory, string uri)
        {
            string actual = CommunAxiom.Commons.Orleans.OrleansConstants.BLANK_ID;
            if (!string.IsNullOrWhiteSpace(uri))
            {
                if (!uri.StartsWith("usr://") && !uri.StartsWith("com://"))
                {
                    throw new ArgumentException("Id should be a uri");
                }
                actual = uri;
            }
            return grainFactory.GetGrain<IUriRegistry>(actual);
        }

        public static async Task<ICommonsActor> GetActor(this IComaxGrainFactory grainFactory, Guid? id = null)
        {
            var uri = (await grainFactory.LoadUser()).Uri;
            var gr = grainFactory.GetGrain<ICommonsActor>(uri);
            return gr;
        }

        public static ICentral GetCentral(this IComaxGrainFactory grainFactory)
        {
            return grainFactory.GetGrain<ICentral>(Guid.Empty);
        }

        public static IPublicBoard GetPublicBoard(this IComaxGrainFactory grainFactory)
        {
            return grainFactory.GetGrain<IPublicBoard>(Guid.Empty);
        }

        public static async Task<ISubjectOfInterest> GetSubjectOfInterest(this IComaxGrainFactory grainFactory)
        {
            Guid userID = (await grainFactory.LoadUser()).InternalId;
            return grainFactory.GetGrain<ISubjectOfInterest>(userID);
        }
    }
}
