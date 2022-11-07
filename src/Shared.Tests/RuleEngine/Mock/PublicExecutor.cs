using CommunAxiom.Commons.Shared.RuleEngine;

namespace Shared.Tests.RuleEngine.Mock
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
