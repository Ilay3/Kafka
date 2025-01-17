using System;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Confluent.Kafka;
using Kafka.Domain.Entities;
using Kafka.Domain.Services;

namespace Kafka.Application.Services
{
    public class KafkaEmailNotificationConsumer : IEmailNotificationConsumer
    {
        private readonly IConsumer<string, string> _consumer;
        private readonly IEmailService _emailService;
        private readonly string _topic;

        public KafkaEmailNotificationConsumer(
            string bootstrapServers,
            string groupId,
            string topic,
            IEmailService emailService)
        {
            _topic = topic;
            _emailService = emailService;

            var config = new ConsumerConfig
            {
                BootstrapServers = bootstrapServers,
                GroupId = groupId,
                AutoOffsetReset = AutoOffsetReset.Earliest
            };

            _consumer = new ConsumerBuilder<string, string>(config).Build();
        }

        public async Task StartAsync(CancellationToken cancellationToken = default)
        {
            _consumer.Subscribe(_topic);

            while (!cancellationToken.IsCancellationRequested)
            {
                try
                {
                    var cr = _consumer.Consume(cancellationToken);
                    var message = cr.Message?.Value;

                    if (string.IsNullOrWhiteSpace(message))
                    {
                        Console.WriteLine("Received empty or null message, skipping...");
                        continue;
                    }

                    try
                    {
                        var notification = JsonSerializer.Deserialize<EmailNotification>(message);
                        if (notification != null)
                        {
                            await _emailService.SendEmailAsync(notification, cancellationToken);
                        }
                    }
                    catch (JsonException ex)
                    {
                        Console.WriteLine($"Invalid JSON message: {message}");
                        Console.WriteLine($"JsonException: {ex.Message}");
                    }

                }
                catch (ConsumeException ex)
                {
                    Console.WriteLine($"Kafka consume error: {ex.Error.Reason}");
                }
                catch (OperationCanceledException)
                {
                    break;
                }
            }

            _consumer.Close();
        }
    }
}
