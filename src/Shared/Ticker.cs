using System;
using System.Collections.Generic;
using System.Text;

namespace CommunAxiom.Commons.Shared
{
    public class Ticker
    {
        private readonly TimeSpan _interval;
        public Ticker(TimeSpan interval) 
        { 
            _interval = interval;
        }

        public async Task<bool> NextTick(CancellationToken cancellationToken)
        {
            await Task.Run(()=>Thread.Sleep(_interval), cancellationToken);
            return !cancellationToken.IsCancellationRequested;
        }

    }
}
