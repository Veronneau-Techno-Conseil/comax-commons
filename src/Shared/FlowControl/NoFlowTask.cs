using System;
using System.Collections.Generic;
using System.Text;

namespace CommunAxiom.Commons.Shared.FlowControl
{
    public class NoFlowTask<StateBag>
    {
        private readonly Func<Task<StateBag>> _setupState;
        private readonly Func<StateBag, Task>? _configure;
        public NoFlowTask(Func<Task<StateBag>> setupState, Func<StateBag, Task>? configure = null) 
        { 
            _setupState = setupState;
            _configure = configure;
        }

        public async Task<TRes> Run<TRes>(Func<StateBag, Task<TRes>> fn) 
        {
            var state = await _setupState();
            return await Task.Run(() =>
            {
                var flow = ExecutionContext.SuppressFlow();
                TRes? res;
                try
                {
                    res = Task.Run(async () =>
                    {
                        if(_configure != null)
                            await _configure(state);
                        return await fn(state);
                    }).ConfigureAwait(true).GetAwaiter().GetResult();
                }
                finally
                {
                    flow.Undo();
                }
                return res;
            });
        }
    }

    public class NoFlowTask
    {
        public static async Task<TRes> Run<TRes>(Func<Task<TRes>> fn)
        {
            return await Task.Run(() =>
            {
                var flow = ExecutionContext.SuppressFlow();
                TRes? res;
                try
                {
                    res = Task.Run(async () =>
                    {
                        return await fn();
                    }).ConfigureAwait(true).GetAwaiter().GetResult();
                }
                finally
                {
                    flow.Undo();
                }
                return res;
            });
        }

        public static async Task Run(Func<Task> fn)
        {
            await Task.Run(() =>
            {
                var flow = ExecutionContext.SuppressFlow();
                try
                {
                    Task.Run(async () =>
                    {
                        await fn();
                    }).ConfigureAwait(true).GetAwaiter().GetResult();
                }
                finally
                {
                    flow.Undo();
                }
            });
        }
    }
}
