using Microsoft.Extensions.Options;
using MimeKit;
using MailKit.Security;
using MailKit.Net.Smtp;
using Microsoft.Extensions.Options;
using System.IO;
using System.Net.Mail;
using MippPortalWebAPI.Models;

namespace MippPortalWebAPI.Helpers
{
    public class MailHelper
    {
        private readonly MailSettings _mailsettings;

        public MailHelper(IOptions<MailSettings> mailSettings) {

            _mailsettings = mailSettings.Value;
        }

        public async Task SendEmailAsync(SendEmailViewModel mailRequest)
        {
            
            var email = new MimeMessage();
            email.Sender = MailboxAddress.Parse(_mailsettings.Mail);
            email.To.Add(MailboxAddress.Parse(mailRequest.ToEmail));
            email.Subject = mailRequest.Subject;
            if (mailRequest.Cc != null && mailRequest.Cc.Count != 0)
            {
                foreach (var item in mailRequest.Cc)
                {
                    email.Cc.Add(MailboxAddress.Parse(item));

                }
            }
            
            var builder = new BodyBuilder();
            
            
            builder.HtmlBody = mailRequest.Body;

            email.Body = builder.ToMessageBody();
            var smtp = new MailKit.Net.Smtp.SmtpClient();
            smtp.Connect(_mailsettings.Host, _mailsettings.Port, SecureSocketOptions.StartTls);
            smtp.Authenticate(_mailsettings.Mail, _mailsettings.Password);
            await smtp.SendAsync(email);
            smtp.Disconnect(true);
        }

    }


}
