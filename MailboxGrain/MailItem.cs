using Comax.Commons.Orchestrator.Contracts.Mailbox;
using CommunAxiom.Commons.Orleans;
using CommunAxiom.Commons.Shared.RuleEngine;
using Newtonsoft.Json.Linq;
using Orleans;
using Orleans.Runtime;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Comax.Commons.Orchestrator.MailboxGrain
{
    internal class MailItem: Grain, IMailItem
    {
        private readonly IPersistentState<Message> _storageState;
        private readonly MailItemRepo _repo;
        public MailItem([PersistentState("mailItemGrain")] IPersistentState<Message> storageState)
        {
            _storageState = storageState;
            _repo = new MailItemRepo(storageState);
        }

        public Task<Message> GetData()
        {
            return _repo.Fetch();
        }

        public Task SaveData(Message value)
        {
            return _repo.Save(value);
        }
    }
}
