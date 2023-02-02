using System;
using System.Collections.Generic;
using System.Text;

namespace CommunAxiom.Commons.Shared.RulesEngine
{
    public static class MessageTypes
    {
        public static class Communicate
        {
            public const string MSG_TYPE_DIRECT = "DIRECT_MSG";
        }

        public static class OrchestratorInstructions
        {
            public const string MSG_TYPE_SYNC_PORTFOLIO = "SYNC_PORTFOLIO";
            public const string MSG_TYPE_ACK_ALIVE = "ACK_ALIVE";
        }

        public static class CommonsAgentEvents
        {
            public const string MSG_TYPE_INGESTION_START = "INGESTION_STARTED";
            public const string MSG_TYPE_INGESTION_END = "INGESTION_ENDED";
            public const string MSG_TYPE_NEW_DATA = "NEW_DATA_VERSION";
        }
    }
}
