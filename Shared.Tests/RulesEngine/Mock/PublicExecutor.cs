using Comax.Commons.Shared.RulesEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Tests.RulesEngine.Mock
{
    public class PublicExecutor : IExecutor<MessageMock>
    {
        public Task Execute(MessageMock param)
        {
            ExecutorTargets.PublicTarget = param;
            return Task.CompletedTask;
        }
    }
}
