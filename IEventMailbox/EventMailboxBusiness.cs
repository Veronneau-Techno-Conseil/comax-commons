
using CommunAxiom.Commons.Orleans;
using Orleans.Runtime;
using Orleans.Streams;
using System;
using System.Threading.Tasks;
using Comax.Commons.Orchestrator.Contracts.EventMailbox;
using CommunAxiom.Commons.Shared;

namespace Comax.Commons.Orchestrator.EventMailboxGrain
{
    public class EventMailboxBusiness
    {
        private Guid? _streamId;
        private IAsyncStream<MailMessage>? _stream;
        private EventMailboxRepo? _eventMailboxRepo;
        private IStreamProvider _streamProvider;

        public EventMailboxBusiness(IStreamProvider streamProvider)
        {
            _streamProvider = streamProvider;
        }

        public void Init(IPersistentState<MailboxState> mbState)
        {
            _eventMailboxRepo = new EventMailboxRepo(mbState);
        }

        private async Task Dispatch(MailMessage mailMessage)
        {
            if (_stream == null)
                throw new InvalidOperationException("Stream cannot be null");
            await _stream.OnNextAsync(mailMessage);
        }

        public async Task<Guid> GetStreamId()
        {
            if(_streamId == null)
                _streamId = Guid.NewGuid();

            if (_stream == null || _stream.Guid != _streamId.Value)
            {
                if(_stream != null)
                {
                    await _stream.OnCompletedAsync();
                }
                _stream = this._streamProvider.GetStream<MailMessage>(_streamId.Value, Constants.DefaultNamespace);
            }

            return _streamId.Value;
        }
      
        public async Task MarkRead(Guid msgId)
        {
            var msg = await _eventMailboxRepo.Find(msgId);
            msg.ReadState = true;
            await _eventMailboxRepo.Save();
        }
        public async Task DeleteMail(Guid msgId)
        {
            await _eventMailboxRepo.Remove(msgId);
        }
        public async Task<OperationResult> ResumeMessageStream()
        {
            if(_streamId == null)
            {
                return new OperationResult
                {
                    IsError = true,
                    Error = EventMailboxConstants.STREAM_NOT_CREATED,
                    Detail = "Retrieve the streamid first"
                };
            }

            if (_stream == null && _streamId != null)
            {
                _stream = this._streamProvider.GetStream<MailMessage>(_streamId.Value, Constants.DefaultNamespace);
            }

            var mailMessages = await _eventMailboxRepo.Fetch();

            _ = Task.Run(async () =>
            {
                var msg = mailMessages;
                foreach (var mail in msg)
                {
                    await Dispatch(mail);
                }
            });

            return new OperationResult();
        }
        public async Task DropMail(MailMessage mail)
        {
            await _eventMailboxRepo.Add(mail);
            
            if(_streamId.HasValue && _stream != null)
                await Dispatch(mail);
        }

        public async Task<bool> HasMail()
        {
            return await _eventMailboxRepo.HasMail();
        }

    }
}
