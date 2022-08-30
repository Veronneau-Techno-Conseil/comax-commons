namespace CommunAxiom.Commons.Shared.RuleEngine
{
    public class Message
    {
        public string From { get; set; }
        public string FromOwner { get; set; }
        public string To { get; set; }
        public string Type { get; set; }
        public string Scope { get; set; }
        public string Payload { get; set; }
        public string Subject { get; set; }
    }
}
