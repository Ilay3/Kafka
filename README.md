# Kafka

**Многоуровневое (DDD) приложение на .NET 8 с использованием Kafka для отправки email.**

## Структура
```bash
Kafka
├── Kafka.Domain
│   ├─ Доменные сущности (Entities)
│   └─ Интерфейсы доменных сервисов (Services)
├── Kafka.Application
│   └─ Логика чтения/публикации (Consumer / Producer) для Kafka
├── Kafka.Infrastructure
│   └─ Инфраструктурные реализации (например, SMTP, FakeEmailService и т.д.)
└── Kafka.Api
    └─ ASP.NET Core Web API + HostedService, который запускает Consumer
```

## Запуск

1. **Установите и запустите Kafka** (классический режим с Zookeeper):
   - Запустите Zookeeper:
     ```bash
     zookeeper-server-start.bat config/zookeeper.properties
     ```
   - Запустите Kafka:
     ```bash
     kafka-server-start.bat config/server.properties
     ```
   - Создайте топик (пример):
     ```bash
     kafka-topics.bat --bootstrap-server localhost:9092
       --create
       --topic email-notifications 
       --partitions 1 
       --replication-factor 1
     ```

2. **Склонируйте** репозиторий и соберите решение:
   ```bash
   git clone https://github.com/<YourName>/Kafka.git
   cd Kafka
   dotnet build

## Настройте Program.cs (в Kafka.Api) при необходимости:

 - kafkaBootstrapServers (по умолчанию localhost:9092)
 - kafkaTopic (по умолчанию email-notifications)
 - kafkaGroupId (по умолчанию email-consumer-group)
 - SMTP-параметры или FakeEmailService.

**Запустите веб-приложение:**

```bash
dotnet run --project Kafka.Api
По умолчанию API будет доступно на http://localhost:5000.
```

##Использование
Отправка письма (Producer) через Web API:

```bash
curl -X POST -H "Content-Type: application/json" \
     -d "{\"Recipient\":\"test@example.com\",\"Subject\":\"Hello\",\"Body\":\"Via Kafka!\"}" \
     http://localhost:5000/api/Email/send
```

В ответ получите:
```bash
{"message":"Email notification posted to Kafka"}
```
Обработка письма (Consumer) происходит автоматически в EmailConsumerHostedService.

Если включён FakeEmailService, в консоли будет что-то вроде:
```bash
(FAKE) Письмо отправлено для: test@example.com ...
```
Если включён SmtpEmailService, письмо уйдёт на реальный адрес (нужны валидные SMTP-реквизиты).

