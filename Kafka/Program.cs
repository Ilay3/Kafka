using Kafka.Application.Services;
using Kafka.Domain.Services;
using Kafka.Infrastructure.Services;

var builder = WebApplication.CreateBuilder(args);

// ===========================
// ������������ (��� ������� - �������)
string kafkaBootstrapServers = "localhost:9092";
string kafkaTopic = "email-notifications";
string kafkaGroupId = "email-consumer-group";

// ���������� ������
// 1. ������������ ���������� ��������� ������� �������� �����:
builder.Services.AddSingleton<IEmailService>(sp =>
{
    // ������� 1: �������� (������ Console.WriteLine)
    return new FakeEmailService();

    // ������� 2: �������� SMTP
    //return new SmtpEmailService(
    //    smtpHost: "smtp.server.com",
    //    smtpPort: 25,
    //    username: "user@server.com",
    //    password: "secret"
    //);
});

// 2. ������������ Kafka-��������
builder.Services.AddSingleton<IEmailNotificationProducer>(sp =>
    new KafkaEmailNotificationProducer(kafkaBootstrapServers, kafkaTopic));

// 3. ������������ Kafka-��������
builder.Services.AddSingleton<IEmailNotificationConsumer>(sp =>
    new KafkaEmailNotificationConsumer(
        kafkaBootstrapServers,
        kafkaGroupId,
        kafkaTopic,
        sp.GetRequiredService<IEmailService>()
    ));

// 4. ��������� HostedService, ����� �������� ������ Kafka
builder.Services.AddHostedService<EmailConsumerHostedService>();

// 5. ��������� �����������
builder.Services.AddControllers();

var app = builder.Build();
app.MapControllers();
app.Run();

// Hosted Service, � ������� ����������� Consumer
public class EmailConsumerHostedService : BackgroundService
{
    private readonly IEmailNotificationConsumer _consumer;

    public EmailConsumerHostedService(IEmailNotificationConsumer consumer)
    {
        _consumer = consumer;
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        return _consumer.StartAsync(stoppingToken);
    }
}
