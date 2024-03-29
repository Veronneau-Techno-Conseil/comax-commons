﻿using System;

namespace CommunAxiom.Commons.CommonsShared.Contracts.EventMailbox
{
    public class MailMessage
    {
        public Guid MsgId { get; set; }
        public string From { get; set; }
        public string To { get; set; }
        public string Type { get; set; }
        public string Subject { get; set; }
        public DateTime ReceivedDate { get; set; }
        public bool ReadState { get; set; }
    }
}
