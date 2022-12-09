using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunAxiom.Commons.Shared.RuleEngine;
using Comax.Commons.Orchestrator.Contracts.ComaxSystem;
using CommunAxiom.Commons.CommonsShared.Contracts.EventMailbox;
using CommunAxiom.Commons.Orleans.Security;

namespace CommunAxiom.Commons.Client.AgentService.OrchClient
{
    public class AgentSyncState : IMailboxObserver, IAsyncDisposable
    {
        private Guid _syncId;
        private IAsyncDisposable _subscription;
        internal string Token { get; private set; }
        public bool? IsAuthorized { get; private set; }
        public Exception PreviousException { get; set; }
        public ConcurrentQueue<Message> Messages { get; private set; } = new ConcurrentQueue<Message>();
        public ConcurrentQueue<MailMessage> MailMessages { get; private set; } = new ConcurrentQueue<MailMessage>();
        public TimeSpan Frequency { get; private set; }
        public TimeSpan SubscriptionTimeout { get; private set; }
        public DateTime LastReceived { get; private set; } = DateTime.MinValue;
        public DateTime NextExecute { get; set; }

        public AgentSyncState(Guid syncId, string token, TimeSpan frequency, TimeSpan subscriptionTimeout)
        {
            Token = token;
            Frequency = frequency;
            Messages = new ConcurrentQueue<Message>();
            SubscriptionTimeout = subscriptionTimeout;
            _syncId = syncId;
        }

        public void Refresh()
        {
            NextExecute = DateTime.UtcNow + Frequency;
        }

        public async Task IAmAlive(IOrchestratorClient client)
        {
            try
            {
                if (_subscription == null || DateTime.UtcNow > (LastReceived + SubscriptionTimeout))
                {
                    if (_subscription != null)
                    {
                        try { await _subscription.DisposeAsync(); } catch { }
                    }
                    AuthStateContext.CurrentAuthState.Value = new AuthStateContext.AuthState() { Token = this.Token };
                    if (!IsAuthorized.HasValue || IsAuthorized.Value)
                    {
                        var mb = await client.GetEventMailbox();
                        _subscription = await mb.Subscribe(this);
                        while (MailMessages.TryDequeue(out var mm))
                        {
                            Messages.Enqueue(await mb.GetMessage(mm.MsgId));
                        }
                    }
                }
            }
            catch(AccessControlException ex)
            {
                IsAuthorized= false;
            }
            finally
            {
                AuthStateContext.CurrentAuthState.Value = AuthStateContext.AuthState.Default;
            }
        }

        public Task NewMail(MailMessage message)
        {
            IsAuthorized = true;
            MailMessages.Enqueue(message);
            LastReceived = DateTime.UtcNow;
            return Task.CompletedTask;
        }

        public async ValueTask DisposeAsync()
        {
            await _subscription.DisposeAsync();
            AgentIntegration.Remove(_syncId);
        }
    }

    public enum SyncStatus
    {
        Connected,
        Disconnected,
        Failed
    }
}
