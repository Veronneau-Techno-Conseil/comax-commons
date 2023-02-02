using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using Orleans;
using CommunAxiom.Commons.Shared;
using Microsoft.Extensions.Logging;

namespace CommunAxiom.Commons.Orleans
{
    public class GrainObserverSubscription<TIObserver> : IAsyncDisposable where TIObserver : IGrainObserver
    {
        private readonly CancellationTokenSource _cancellation = new();
        private readonly WeakReference _watcher;
        private readonly Task _watcherTask;
        private readonly TIObserver _watcherReference;
        private readonly Func<TIObserver, Task> _subscribe;
        private readonly Func<TIObserver, Task> _unsubscribe;
        private readonly ILogger _logger;

        public GrainObserverSubscription(TIObserver watcher, TIObserver watcherReference, Func<TIObserver, Task> subscribe, Func<TIObserver, Task> unsubscribe, ILogger logger)
        {
            _watcher = new WeakReference(watcher);
            _watcherReference = watcherReference;
            _subscribe = subscribe;
            _unsubscribe = unsubscribe;
            _logger = logger;
            _watcherTask = Task.Run(Watch);
        }

        private async Task Watch()
        {
            var ticker = new Ticker(TimeSpan.FromSeconds(30));
            
            await _subscribe(_watcherReference);
            while (await ticker.NextTick(_cancellation.Token))
            {
                // When the client disconnects, the .NET garbage collector can clean up the watcher object.
                // When that happens, we will stop watching.
                // Until then, periodically heartbeat the poll grain to let it know we're still watching.
                if (_watcher.IsAlive)
                {
                    try
                    {
                        await _subscribe(_watcherReference);
                    }
                    catch(Exception e)
                    {
                        _logger.LogError(e, "Error calling subscribe method");
                    }
                }
                else
                {
                    // The poll watcher object has been cleaned up, so stop refreshing its subscription.
                    break;
                }
            }

            // Notify the poll grain that we are no longer interested
            _unsubscribe(_watcherReference).Ignore();
        }

        public async ValueTask DisposeAsync()
        {
            _cancellation.Cancel();
            try
            {
                await _watcherTask;
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "Error completing observer watcher task");
            }

            _cancellation.Dispose();
        }
    }
}
