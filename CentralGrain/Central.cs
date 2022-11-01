using Comax.Commons.Orchestrator.Contracts.Central;
using CommunAxiom.Commons.Orleans.Security;
using Microsoft.Extensions.DependencyInjection;
using Orleans;
using Orleans.Concurrency;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Comax.Commons.Orchestrator.CentralGrain
{
    [StatelessWorker(1)]
    [Reentrant]
    public class Central : Grain,  ICentral
    {
        private readonly IServiceProvider _serviceProvider;

        public Central(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        [AuthorizeClaim(ClaimType = "https://orchestrator.communaxiom.org/mycontacts", ClaimValueFilter = "read", MatchStrategy = AuthorizeClaimAttribute.FilterMode.Equals)]
        public async Task<IEnumerable<Contact>?> GetContacts()
        {
            var fac = _serviceProvider.GetService<SvcFactory>();
            var svc = await fac.GetService();
            var contacts = await svc.GetContactsAsync();
            if (contacts == null)
                return new Contact[0];
            return contacts.Select(x => 
                new Contact
                {
                    Id = x.Id,
                    UserName = x.UserName
                }).ToArray();
        }

        [AuthorizeClaim(ClaimType = "https://orchestrator.communaxiom.org/mycontacts", ClaimValueFilter = "read", MatchStrategy = AuthorizeClaimAttribute.FilterMode.Equals)]
        public async Task<bool> CanMessage(string userId)
        {
            var fac = _serviceProvider.GetService<SvcFactory>();
            var svc = await fac.GetService();
            var contacts = await svc.GetContactsAsync();
            return contacts.Any(x => x.Id == userId);
        }
    }
}
