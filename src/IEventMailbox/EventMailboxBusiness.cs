
using CommunAxiom.Commons.Orleans;
using Orleans.Runtime;
using Orleans.Streams;
using System;
using System.Threading.Tasks;
using CommunAxiom.Commons.Shared;
using CommunAxiom.Commons.CommonsShared.Contracts.EventMailbox;
using CommunAxiom.Commons.CommonsShared.Contracts.Mail;
using Microsoft.Extensions.Logging;
using CommunAxiom.Commons.Shared.RuleEngine;
using CommunAxiom.Commons.Orleans.Security;
using CommunAxiom.Commons.Shared.RulesEngine;
using System.Collections.Generic;

namespace CommunAxiom.Commons.CommonsShared.EventMailboxGrain
{
    public class EventMailboxBusiness
    {
        private Guid? _streamId;
        private IAsyncStream<MailMessage>? _stream;
        private EventMailboxRepo? _eventMailboxRepo;
        private IComaxGrainFactory _comaxGrainFactory;
        private ObserverManager<IMailboxObserver> _mailboxObservers;
        private readonly ILogger _logger;
        private List<Guid> _idsToRemove = new List<Guid>();

        public EventMailboxBusiness(IComaxGrainFactory comaxGrainFactory, ILogger logger)
        {
            
            _comaxGrainFactory = comaxGrainFactory;
            _logger = logger;
            _mailboxObservers = new ObserverManager<IMailboxObserver>(TimeSpan.FromMinutes(5), logger, "EvtMBObs_");
        }

        public async Task Subscribe(IMailboxObserver mailboxObserver)
        {
            _mailboxObservers.Subscribe(mailboxObserver, mailboxObserver);

            var mailMessages = await _eventMailboxRepo.Fetch();

            _ = Task.Run(async () =>
            {
                var msg = mailMessages;
                foreach (var mail in msg)
                {
                    await Dispatch(mail);
                    Console.WriteLine("Dispatched Mail");
                }
            }).ContinueWith(ac =>
            {
                if (ac.IsFaulted)
                    Console.Error.WriteLine(ac.Exception);
            });
        }

        public async Task StreamMails(StreamSpec streamSpec)
        {
            var sp = _comaxGrainFactory.GetStreamProvider(streamSpec.StreamProvider);
            var stream = sp.GetStream<MailMessage>(streamSpec.Id, streamSpec.Namespace);
            var mailMessages = await _eventMailboxRepo.Fetch();

            _ = Task.Run(async () =>
            {
                var msg = mailMessages;
                foreach (var mail in msg)
                {
                    await stream.OnNextAsync(mail);
                    if(mail.Type == MessageTypes.OrchestratorInstructions.MSG_TYPE_ACK_ALIVE)
                    {
                        _idsToRemove.Add(mail.MsgId);
                    }
                }
                await stream.OnCompletedAsync();
            }).ContinueWith(ac =>
            {
                if (ac.IsFaulted)
                    Console.Error.WriteLine(ac.Exception);
            });
        }

        public async Task<int> Cleanup()
        {
            int res = 0;
            foreach (var item in _idsToRemove)
            {
                await _eventMailboxRepo.Remove(item);
                res++;
            }
            return res;
        }

        public Task Unsubscribe(IMailboxObserver mailboxObserver)
        {
            _mailboxObservers.Unsubscribe(mailboxObserver);
            return Task.CompletedTask;
        }

        public void Init(IPersistentState<MailboxState> mbState)
        {
            _eventMailboxRepo = new EventMailboxRepo(mbState);
        }

        private Task Dispatch(MailMessage mailMessage)
        {
            _mailboxObservers.Notify(mo => mo.NewMail(mailMessage));
            return Task.CompletedTask;
        }

        public async Task MarkRead(Guid msgId)
        {
            var msg = await _eventMailboxRepo.Find(msgId);
            msg.ReadState = true;
            await _eventMailboxRepo.Save();
        }
        public async Task DeleteMail(Guid msgId)
        {
            var mail = this._comaxGrainFactory.GetGrain<IMail>(msgId);
            await mail.Delete();
            await _eventMailboxRepo.Remove(msgId);
        }
        
        public async Task DropMail(MailMessage mail)
        {
            await _eventMailboxRepo.Add(mail);

            if (_streamId.HasValue && _stream != null)
                await Dispatch(mail);
        }

        public async Task<bool> HasMail()
        {
            return await _eventMailboxRepo.HasMail();
        }

        public async Task<Message> GetMessage(Guid msgId)
        {
            var message = await _eventMailboxRepo.Find(msgId);
            if (message == null)
                return null;
            var mg = this._comaxGrainFactory.GetGrain<IMail>(msgId);
            return await mg.GetMessage();

        }
    }
}
