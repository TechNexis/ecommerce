
//using API.IEmailSettings;
using API.Settings;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;

namespace API.MailSettingsService
{
    public class EmailSettings(IOptions<MailSettings> options, ILogger<EmailSettings> logger) : IEmailSettings
    {
        private readonly MailSettings _options = options.Value;
        private readonly ILogger<EmailSettings> _logger = logger;

        public async Task SendEmailMessage(Email email)
        {
            var mail = new MimeMessage
            {
                Sender = MailboxAddress.Parse(_options.Email),
                Subject = email.Subject
            };

            mail.From.Add(new MailboxAddress("Aya", _options.Email));///********
            mail.To.Add(new MailboxAddress("User", email.To));

            var builder = new BodyBuilder
            {
                HtmlBody = email.Body
            };
            mail.Body = builder.ToMessageBody();

            using var smtp = new SmtpClient();///service to send the message 

            try
            {
                await smtp.ConnectAsync(_options.Host, _options.Port, SecureSocketOptions.SslOnConnect);//4
               // await smtp.ConnectAsync(_options.Host, _options.Port, SecureSocketOptions.StartTls);//5

                await smtp.AuthenticateAsync(_options.Email, _options.Password);
                await smtp.SendAsync(mail);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while sending email.");
                throw;
            }
            finally
            {
                await smtp.DisconnectAsync(true);
            }
        }



    }
}