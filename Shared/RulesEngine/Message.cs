namespace CommunAxiom.Commons.Shared.RuleEngine
{
    public class Message
    {
        public Guid Id { get; set; }
        public string From { get; set; }
        public string FromOwner { get; set; }
        public string To { get; set; }
        public string Type { get; set; }
        public string Scope { get; set; }
        public string Payload { get; set; }
        public string Subject { get; set; }
        public DateTime CreatedDate { get; set; }

        public List<MessageLogEntry> TransferLog { get; set; } = new List<MessageLogEntry>();
    }

    public class MessageLogEntry
    {
        public string Recipient { get; set; }
        public DateTime Received { get; set; }
    }
}
