using CommunAxiom.Commons.Shared.RuleEngine;
using Newtonsoft.Json.Linq;
using Orleans.Runtime;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Comax.Commons.Orchestrator.MailboxGrain
{
    public class MailItemRepo
    {
        private readonly IPersistentState<Message> _state;
        public MailItemRepo(IPersistentState<Message> state)
        {
            _state = state;
        }

        public async Task<Message> Fetch()
        {
            await _state.ReadStateAsync();
            return _state.State;
        }

        public async Task Save(Message value)
        {
            _state.State = value;
            await _state.WriteStateAsync();
        }
    }
}
