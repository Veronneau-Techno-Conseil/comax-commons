using Microsoft.Extensions.Logging;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Text;
using System.Threading;

namespace CommunAxiom.Commons.Shared.FlowControl
{
    public class SegregatedContext<StateBag> : IDisposable where StateBag: class
    {
        private bool _disposed = false;
        private ConcurrentQueue<WorkItem> _incomingTasks = new ConcurrentQueue<WorkItem>();
        private readonly CancellationToken _cancellationToken;
        private readonly Func<Task<StateBag>> _setupState;
        private readonly TimeSpan timeout = TimeSpan.FromMinutes(2);
        private readonly ILogger _logger;
        private readonly TaskCompletionSource<bool> _isReady = new TaskCompletionSource<bool>();



        public SegregatedContext(Func<Task<StateBag>> setupState, CancellationToken cancellationToken, ILogger logger)
        {
            _setupState = setupState;
            _cancellationToken = cancellationToken;
            _logger = logger;
            _ = this.MainLoop();
        }

        public Task<bool> WaitForContext()
        {
            return _isReady.Task;
        }

        public Task<TReturn> Run<TReturn>(Func<StateBag, Task<TReturn>> toExec, TimeSpan? timeout = null)
        {
            if (_disposed)
                throw new ObjectDisposedException("Cannot run task on disposed context");
            TaskCompletionSource<TReturn> result = new TaskCompletionSource<TReturn>();
            CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();

            WorkItem wi = new WorkItem
            {
                CancellationTokenSource = cancellationTokenSource,
                Job = async (bag) =>
                {
                    await Task.Run(async () =>
                    {
                        if (!_cancellationToken.IsCancellationRequested &&
                            (result.Task.Status == TaskStatus.WaitingForActivation
                                || result.Task.Status == TaskStatus.Running))
                        {
                            try
                            {
                                var res = await toExec(bag);
                                result.SetResult(res);
                            }
                            catch (Exception ex)
                            {
                                result.SetException(ex);
                            }
                        }
                    }, _cancellationToken);
                },
                TimeoutTask = Task.Delay(timeout ?? TimeSpan.FromMinutes(2), cancellationTokenSource.Token).ContinueWith(t =>
                {
                    cancellationTokenSource.Cancel();
                    if (result.Task.Status == TaskStatus.WaitingForActivation || result.Task.Status == TaskStatus.Running)
                        result.SetException(new TimeoutException("Wait time for worker elasped"));
                })
            };

            _incomingTasks.Enqueue(wi);
            return result.Task;
        }

        public Task Run(Func<StateBag, Task> toExec, TimeSpan? timeout = null)
        {
            if (_disposed)
                throw new ObjectDisposedException("Cannot run task on disposed context");
            TaskCompletionSource<bool> result = new TaskCompletionSource<bool>();
            CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();

            WorkItem wi = new WorkItem
            {
                CancellationTokenSource = cancellationTokenSource,
                Job = async (bag) =>
                {
                    await Task.Run(async () =>
                    {
                        if (!_cancellationToken.IsCancellationRequested &&
                            (result.Task.Status == TaskStatus.WaitingForActivation
                                || result.Task.Status == TaskStatus.Running))
                        {
                            try
                            {
                                await toExec(bag);
                                result.SetResult(true);
                            }
                            catch(Exception ex)
                            {
                                result.SetException(ex);
                            }
                        }
                    }, _cancellationToken);
                },
                TimeoutTask = Task.Delay(timeout ?? TimeSpan.FromMinutes(2), cancellationTokenSource.Token).ContinueWith(t =>
                {
                    cancellationTokenSource.Cancel();
                    if (result.Task.Status == TaskStatus.WaitingForActivation || result.Task.Status == TaskStatus.Running)
                        result.SetException(new TimeoutException("Wait time for worker elasped"));
                })
            };

            _incomingTasks.Enqueue(wi);
            return result.Task;
        }


        public Task MainLoop()
        {
            return Task.Run(async () =>
            {
                var flow = ExecutionContext.SuppressFlow();
                Task.Run(async () =>
                {
                    StateBag? stateBag = null;
                    try
                    {
                        stateBag = await _setupState();
                        _isReady.SetResult(true);
                    }
                    catch(Exception e)
                    {
                        _isReady.SetException(e);
                    }

                    if(stateBag == null)
                    {
                        _isReady.SetResult(false);
                        return;
                    }

                    while (!_cancellationToken.IsCancellationRequested && !_disposed)
                    {
                        try
                        {
                            if (_incomingTasks.TryDequeue(out var work))
                            {
                                _ = Task.Run(async () =>
                                {
                                    await work.Job(stateBag);
                                    work.CancellationTokenSource.Cancel();
                                });
                            }
                            else
                            {
                                await Task.Delay(200);
                            }
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError(ex, "Error trying to run NoFlowContext");
                        }
                    }
                }).ConfigureAwait(true).GetAwaiter().GetResult();
                flow.Undo();
            });
        }

        public void Dispose()
        {
            while (_incomingTasks.TryDequeue(out WorkItem workItem))
            {
                workItem.CancellationTokenSource.Cancel();
            }
            _disposed = true;
        }


        public class WorkItem
        {
            public CancellationTokenSource CancellationTokenSource { get; set; }
            public Func<StateBag, Task> Job { get; set; }
            public Task TimeoutTask { get; set; }
        }
    }
}
