namespace Kafka.Domain.Entities
{
    public class EmailNotification
    {
        public Guid Id { get; }
        public string Recipient { get; }
        public string Subject { get; }
        public string Body { get; }

        public EmailNotification(Guid id, string recipient, string subject, string body)
        {
            Id = id;
            Recipient = recipient;
            Subject = subject;
            Body = body;
        }
    }
}
