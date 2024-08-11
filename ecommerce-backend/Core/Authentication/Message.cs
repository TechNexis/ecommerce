using MimeKit;

namespace Core.Authentication
{
    public class Message(IEnumerable<string> to, string subject, string content)
    {
        public List<MailboxAddress> To { get; set; } = to.Select(x => new MailboxAddress("email", x)).ToList();
        public string Subject { get; set; } = subject;
        public string Content { get; set; } = content;
    }
}
