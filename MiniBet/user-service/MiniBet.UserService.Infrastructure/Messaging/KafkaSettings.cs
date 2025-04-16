using System;

namespace MiniBet.UserService.Infrastructure.Messaging;

public class KafkaSettings
{
    public required string BootstrapServers { get; set; }

    public required string UsersTopic { get; set; }
}
