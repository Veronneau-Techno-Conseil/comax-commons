using Orleans.Streams;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CommunAxiom.Commons.Shared
{
    public class AsyncEnumerableStream<TResult> : IAsyncObserver<TResult>, IAsyncEnumerable<TResult>, IAsyncEnumerator<TResult>
    {
        private System.Collections.Concurrent.ConcurrentQueue<TResult> _queue = new System.Collections.Concurrent.ConcurrentQueue<TResult>();
        private bool _isCompleted = false;
        public bool IsCompleted { get { return _isCompleted; } }
        private TResult? _current;
        private CancellationToken _cancellationToken;

        public TResult Current => _current ?? throw new EndOfStreamException();

        public ValueTask DisposeAsync()
        {
            _queue = null;
            _isCompleted = true;
            return new ValueTask(Task.CompletedTask);
        }

        public IAsyncEnumerator<TResult> GetAsyncEnumerator(CancellationToken cancellationToken = default)
        {
            _cancellationToken = cancellationToken;
            return this;
        }

        public async ValueTask<bool> MoveNextAsync()
        {
            //While token not cancelled
            while (!_cancellationToken.IsCancellationRequested)
            {
                if (_queue.IsEmpty)
                {
                    //queue is empty
                    if (_isCompleted)
                    {
                        //i'm done
                        return false;
                    }
                    //i should wait
                    await Task.Delay(250);
                    continue;
                }
                else
                {
                    //queue does not look empty
                    if (_queue.TryDequeue(out TResult? next))
                    {
                        //set new current
                        _current = next;
                        return true;
                    }
                }
            }
            return false;
        }

        /// <summary>
        /// Stream if finished, should end the enumeration
        /// </summary>
        /// <returns></returns>
        public Task OnCompletedAsync()
        {
            _isCompleted = true;
            return Task.CompletedTask;
        }

        /// <summary>
        /// TODO: find out what to do with this...
        /// </summary>
        /// <param name="ex"></param>
        /// <returns></returns>
        public Task OnErrorAsync(Exception ex)
        {
            throw ex;
        }

        /// <summary>
        /// Stream coming in, adding to the queue
        /// </summary>
        /// <param name="item"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public Task OnNextAsync(TResult item, StreamSequenceToken token = null)
        {
            _queue.Enqueue(item);
            return Task.CompletedTask;
        }
    }
}
