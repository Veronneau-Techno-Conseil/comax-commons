namespace Shared.Tests.RuleEngine.Mock
{
    public class MailMock
    {
        public Guid MsgId { get; set; }
        public string From { get; set; }
        public string Type { get; set; }
        public string Subject { get; set; }
        public DateTime ReceivedDate { get; set; }
        public bool ReadState { get; set; }
    }
}
