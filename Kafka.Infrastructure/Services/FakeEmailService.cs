using System;
using System.Threading;
using System.Threading.Tasks;
using Kafka.Domain.Entities;
using Kafka.Domain.Services;

namespace Kafka.Infrastructure.Services
{
    public class FakeEmailService : IEmailService
    {
        public Task SendEmailAsync(EmailNotification notification, CancellationToken cancellationToken = default)
        {
            // Просто выводим в консоль (или лог) информацию
            Console.WriteLine($"(FAKE) Письмо отправлено для: {notification.Recipient}, " +
                              $"Тема: {notification.Subject}, Текст: {notification.Body}");
            return Task.CompletedTask;
        }
    }
}
