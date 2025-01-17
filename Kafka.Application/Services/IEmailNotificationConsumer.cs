using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kafka.Application.Services;

public interface IEmailNotificationConsumer
{
    Task StartAsync(CancellationToken cancellationToken = default);
}