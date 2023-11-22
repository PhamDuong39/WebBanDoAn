using MailKit.Net.Smtp;
using MimeKit;
using WebBanDoAn.IServices;
using WebBanDoAn.ViewModels.AuthModels;
using WebBanDoAn.ViewModels.ResponseModel;

namespace WebBanDoAn.Services
{
    public class EmailServices : IEmailServices
    {
        // Getting all the config in the appsetting
        private readonly EmailConfiguration _emailConfig;

        public EmailServices(EmailConfiguration emailConfig)
        {
            _emailConfig = emailConfig;
        }

        public void SendEmail(EmailMessageSetUpModel emailMessageSetUp)
        {
            var message = CreateEmailMessage(emailMessageSetUp);
            Send(message);
        }

        private MimeMessage CreateEmailMessage(EmailMessageSetUpModel emailMessageSetUp)
        {
            var emailMessage = new MimeMessage();
            emailMessage.From.Add(new MailboxAddress("email", _emailConfig.From));
            emailMessage.To.AddRange(emailMessageSetUp.To);
            emailMessage.Subject = emailMessageSetUp.Subject;
            emailMessage.Body = new TextPart(MimeKit.Text.TextFormat.Html)
            {
                Text = emailMessageSetUp.Content
            };

            return emailMessage;
        }

        private void Send(MimeMessage mailMessage)
        {
            using var client = new SmtpClient();
            try
            {
                client.Connect(_emailConfig.SmtpServer, _emailConfig.Port, true);
                client.AuthenticationMechanisms.Remove("XOAUTH2");
                client.Authenticate(_emailConfig.UserName, _emailConfig.Password);

                client.Send(mailMessage);
            }
            catch (Exception)
            {

                throw;
            }
            finally
            {
                client.Disconnect(true);
                client.Dispose();
            }
        }
    }
}
