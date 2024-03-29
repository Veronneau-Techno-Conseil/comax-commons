﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CommunAxiom.Commons.CommonsShared.Contracts.Mail;
using CommunAxiom.Commons.Shared.RuleEngine;
using Orleans;
using Orleans.Runtime;
using Orleans.Streams;

namespace Comax.Commons.Orchestrator.MailGrain
{
    public class Mail : Grain, IMail
    {
        private IPersistentState<Message> _storageState;
        public Mail([PersistentState("Message")] IPersistentState<Message> storageState)
        {
            _storageState = storageState;
        }

        public async Task Delete()
        {
            if (_storageState.RecordExists)
            {
                await _storageState.ReadStateAsync();
                await _storageState.ClearStateAsync();
            }
        }

        public async Task<bool> Exists()
        {
            await _storageState.ReadStateAsync();
            return _storageState.State != null;
        }

        public async Task<Message> GetMessage()
        {
            await _storageState.ReadStateAsync();
            return _storageState.State;
        }

        public async Task Save(Message message)
        {
            await _storageState.ReadStateAsync();
            _storageState.State = message;
            await _storageState.WriteStateAsync();
        }
    }

}
