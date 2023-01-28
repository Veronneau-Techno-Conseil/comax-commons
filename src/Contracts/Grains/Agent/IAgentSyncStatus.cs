using CommunAxiom.Commons.CommonsShared.Contracts.EventMailbox;
using CommunAxiom.Commons.Shared.RuleEngine;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;

namespace CommunAxiom.Commons.Client.Contracts.Grains.Agent
{
    public interface IAgentSyncStatus
    {
        bool? IsAuthorized { get; set; }
        bool IsConnected { get; set; }
        Exception PreviousException { get; set; }
        ConcurrentQueue<Message> Messages { get; set; }
        ConcurrentQueue<MailMessage> MailMessages { get; set; }
        DateTime LastReceived { get; set; }
        DateTime LastActive { get; set; }
        string? Token { get; }
    }
}
