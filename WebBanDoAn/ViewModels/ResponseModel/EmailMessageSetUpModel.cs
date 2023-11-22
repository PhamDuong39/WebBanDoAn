using MimeKit;

namespace WebBanDoAn.ViewModels.ResponseModel
{
    public class EmailMessageSetUpModel
    {
        public List<MailboxAddress> To { get; set; }
        public string? Subject { get; set; }
        public string? Content { get; set; }
        public EmailMessageSetUpModel(IEnumerable<string> to, string subject, string content)
        {
            To = new List<MailboxAddress>();
            To.AddRange(to.Select(p => new MailboxAddress("email", p)));
            Subject = subject;
            Content = content;
        }
    }
}
