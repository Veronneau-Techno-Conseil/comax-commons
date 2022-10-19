using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Comax.Commons.Orchestrator.Contracts.Mail;
using Orleans;
using Orleans.Runtime;
using Orleans.Streams;

namespace Comax.Commons.Orchestrator.MailGrain
{
    public class Mail:Grain,IMail
    {
        private IPersistentState<MailMessage> _storageState;
        public Mail([PersistentState("mailGrain")] IPersistentState<MailMessage> storageState)
        {
            _storageState = storageState;
        }


        public async Task<bool> HasMail()
        {
            await _storageState.ReadStateAsync();

            return !string.IsNullOrWhiteSpace(_storageState.State?.MsgId.ToString());
        }

        public async Task ResumeMessageStream()
        {
            throw new NotImplementedException();
        }
    }

    
}
