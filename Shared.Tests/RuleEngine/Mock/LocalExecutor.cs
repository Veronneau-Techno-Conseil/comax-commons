using CommunAxiom.Commons.Shared.RuleEngine;

namespace Shared.Tests.RuleEngine.Mock
{
    public class LocalExecutor : IExecutor<MessageMock>
    {
        public Task Execute(MessageMock param)
        {
            ExecutorTargets.LocalTarget = param;
            return Task.CompletedTask;
        }
    }
}
