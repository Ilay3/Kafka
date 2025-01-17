using Kafka.Domain.Entities;

namespace Kafka.Domain.Services;
public interface IEmailService
{
    Task SendEmailAsync(EmailNotification notification, CancellationToken cancellationToken = default);
}
