using MailKit.Net.Smtp;
using MimeKit;
using ParkLink.Identity.Dtos;

namespace ParkLink.Identity.Services.Emails
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _configuration;

        public EmailService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public void SendEmailToUser(EmailDto dto)
        {
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress("", dto.smtpFrom));
            message.To.Add(new MailboxAddress("", dto.strUserEmail));
            message.Subject = dto.strSubject;
            message.Body = new BodyBuilder
            {
                HtmlBody = dto.strMessage
            }.ToMessageBody();

            bool bSslValue = Convert.ToBoolean(_configuration.GetSection("EmailConfiguration").GetSection("UseSSL").Value);
            Boolean blnSSl1;

            using (var client = new SmtpClient())
            {
                if (bSslValue is false)
                {
                    blnSSl1 = false;
                }
                else
                {
                    blnSSl1 = true;
                }

                client.ServerCertificateValidationCallback = (s, c, h, e) => true;
                client.Connect(dto.smtpServer, dto.smtPort, blnSSl1);
                client.Authenticate(dto.smtpUser, dto.smtPwd);
                client.Send(message);
                client.Disconnect(true);
            }
        }

        public async Task SendPasswordResetEmailAsync(string email, string firstName, string resetUrl)
        {
            throw new NotImplementedException();
        }
    }
}
