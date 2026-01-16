namespace ClassLibrary
{
    public class HelpTicket(string content, Admin recipient, Creator sender)
    {
        public Admin Recipient { get; } = recipient;
        public Creator Sender { get; } = sender;
        public string Content { get; } = content;
    }
}
