using System.Threading;
using System.Threading.Tasks;
using Kafka.Domain.Entities;
using Kafka.Domain.Services;
using MailKit.Net.Smtp;
using MimeKit;

namespace Kafka.Infrastructure.Services
{
    public class SmtpEmailService : IEmailService
    {
        private readonly string _smtpHost;
        private readonly int _smtpPort;
        private readonly string _username;
        private readonly string _password;

        public SmtpEmailService(string smtpHost, int smtpPort, string username, string password)
        {
            _smtpHost = smtpHost;
            _smtpPort = smtpPort;
            _username = username;
            _password = password;
        }

        public async Task SendEmailAsync(EmailNotification notification, CancellationToken cancellationToken = default)
        {
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress("NoReply", _username));
            message.To.Add(new MailboxAddress(notification.Recipient, notification.Recipient));
            message.Subject = notification.Subject;
            message.Body = new TextPart("plain") { Text = notification.Body };

            using var client = new SmtpClient();
            await client.ConnectAsync(_smtpHost, _smtpPort, useSsl: false, cancellationToken);
            await client.AuthenticateAsync(_username, _password, cancellationToken);
            await client.SendAsync(message, cancellationToken);
            await client.DisconnectAsync(true, cancellationToken);
        }
    }
}
