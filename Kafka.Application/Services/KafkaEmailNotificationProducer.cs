using Confluent.Kafka;
using Kafka.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Kafka.Application.Services
{
    public class KafkaEmailNotificationProducer : IEmailNotificationProducer
    {
        private readonly IProducer<string, string> _producer;
        private readonly string _topic;

        public KafkaEmailNotificationProducer(string bootstrapServers, string topic)
        {
            _topic = topic;
            var config = new ProducerConfig
            {
                BootstrapServers = bootstrapServers,
            };
            _producer = new ProducerBuilder<string, string>(config).Build();
        }

        public async Task ProduceAsync (EmailNotification notification, CancellationToken cancellationToken = default)
        {
            var value = JsonSerializer.Serialize(notification);
            var message = new Message<string, string>
            {
                Key = notification.Id.ToString(),
                Value = value
            };
            await _producer.ProduceAsync(_topic, message, cancellationToken);
        }
    }
}
