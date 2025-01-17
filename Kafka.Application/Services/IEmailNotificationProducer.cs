﻿using Kafka.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kafka.Application.Services;

public interface IEmailNotificationProducer
{
    Task ProduceAsync(EmailNotification notification, CancellationToken cancellationToken = default);
}