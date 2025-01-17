using System;
using System.Threading;
using System.Threading.Tasks;
using Kafka.Application.Services;
using Kafka.Domain.Entities;
using Microsoft.AspNetCore.Mvc;

namespace Kafka.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EmailController : ControllerBase
    {
        private readonly IEmailNotificationProducer _producer;

        public EmailController(IEmailNotificationProducer producer)
        {
            _producer = producer;
        }

        [HttpPost("send")]
        public async Task<IActionResult> SendEmail([FromBody] EmailDto dto, CancellationToken cancellationToken)
        {
            var notification = new EmailNotification(
                Guid.NewGuid(),
                dto.Recipient,
                dto.Subject,
                dto.Body
            );

            // Публикуем сообщение в Kafka
            await _producer.ProduceAsync(notification, cancellationToken);

            return Ok(new { Message = "Email notification posted to Kafka" });
        }
    }

    public record EmailDto(string Recipient, string Subject, string Body);
}
