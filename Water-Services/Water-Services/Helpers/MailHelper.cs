using MailKit.Net.Smtp;
using Microsoft.Extensions.Configuration;
using MimeKit;

namespace Water_Services.Helpers
{
    public class MailHelper : IMailHelper
    {
        private readonly IConfiguration _configuration;

        public MailHelper(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public bool SendEmail(string receiver, string subject, string body)
        {
            var senderName = _configuration["Mail:SenderName"];
            var senderEmail = _configuration["Mail:SenderEmail"];
            var password = _configuration["Mail:Password"];
            var smtp = _configuration["Mail:Smtp"];
            var port = _configuration["Mail:Port"];

            var message = new MimeMessage();
            message.From.Add(new MailboxAddress(senderName, senderEmail));
            message.To.Add(new MailboxAddress(receiver, receiver));
            message.Subject = subject;

            var bodyBuilder = new BodyBuilder
            {
                HtmlBody = body
            };

            message.Body = bodyBuilder.ToMessageBody();

            try
            {
                using (var client = new SmtpClient())
                {
                    client.Connect(smtp, int.Parse(port), false);
                    client.Authenticate(senderEmail, password);
                    client.Send(message);
                    client.Disconnect(true);
                }

                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
