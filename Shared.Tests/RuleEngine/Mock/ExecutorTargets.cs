namespace Shared.Tests.RuleEngine.Mock
{
    public static class ExecutorTargets
    {
        public static MessageMock LocalTarget { get; set; }
        public static MessageMock PublicTarget { get; set; }

        public static void Flush()
        {
            LocalTarget = null;
            PublicTarget = null;
        }
    }
}
