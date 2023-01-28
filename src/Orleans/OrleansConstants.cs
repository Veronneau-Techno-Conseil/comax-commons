using System;
using System.Collections.Generic;
using System.Text;

namespace CommunAxiom.Commons.Orleans
{
    public static class OrleansConstants
    {
        public static class Streams
        {
            public const string DefaultStream = "Default";
            public const string ImplicitStream = "Implicit";
        }
        
        public static class StreamNamespaces
        {
            public const string DefaultNamespace = "Default";
            public const string MAILBOX_STREAM_INBOUND_NS = "MAILBOX_STREAM_INBOUND";
            public const string PORTFOLIO_NOTIFS_NS = "PORTFOLIO_NOTIFS_NS";
            public const string MAILBOX_SUB_NS = "MAILBOX_SUB_NS";
        }
        
        public const string BLANK_ID = "BLANK";

        public static class Storage
        {
            public const string PubSubStore = "PubSubStore";
            public const string JObjectStore = "JObjectStore";
        }
    }
}
