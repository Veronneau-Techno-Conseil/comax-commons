using CommunAxiom.Commons.Shared.RuleEngine;

namespace CommunAxiom.Commons.Client.Grains.BroadcastGrain
{
    public class ExecutorTargets
    {
        public static Message LocalTarget { get; set; }
        public static Message PublicTarget { get; set; }

        public static void Flush()
        {
            LocalTarget = null;
            PublicTarget = null;
        }
    }
}
