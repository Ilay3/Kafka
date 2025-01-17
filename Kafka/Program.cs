using Kafka.Application.Services;
using Kafka.Domain.Services;
using Kafka.Infrastructure.Services;

var builder = WebApplication.CreateBuilder(args);

// ===========================
// Конфигурация (для примера - хардкод)
string kafkaBootstrapServers = "localhost:9092";
string kafkaTopic = "email-notifications";
string kafkaGroupId = "email-consumer-group";

// Подключаем службы
// 1. Регистрируем реализацию доменного сервиса отправки писем:
builder.Services.AddSingleton<IEmailService>(sp =>
{
    // Вариант 1: Заглушка (просто Console.WriteLine)
    return new FakeEmailService();

    // Вариант 2: Реальный SMTP
    //return new SmtpEmailService(
    //    smtpHost: "smtp.server.com",
    //    smtpPort: 25,
    //    username: "user@server.com",
    //    password: "secret"
    //);
});

// 2. Регистрируем Kafka-продюсер
builder.Services.AddSingleton<IEmailNotificationProducer>(sp =>
    new KafkaEmailNotificationProducer(kafkaBootstrapServers, kafkaTopic));

// 3. Регистрируем Kafka-консюмер
builder.Services.AddSingleton<IEmailNotificationConsumer>(sp =>
    new KafkaEmailNotificationConsumer(
        kafkaBootstrapServers,
        kafkaGroupId,
        kafkaTopic,
        sp.GetRequiredService<IEmailService>()
    ));

// 4. Добавляем HostedService, чтобы консюмер слушал Kafka
builder.Services.AddHostedService<EmailConsumerHostedService>();

// 5. Добавляем контроллеры
builder.Services.AddControllers();

var app = builder.Build();
app.MapControllers();
app.Run();

// Hosted Service, в котором запускается Consumer
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
