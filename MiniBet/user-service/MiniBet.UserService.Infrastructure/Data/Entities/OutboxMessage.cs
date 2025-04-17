using System;

namespace MiniBet.UserService.Infrastructure.Data.Entities;

public class OutboxMessage
{
    public Guid Id { get; set; }

    public required string EventType { get; set; }

    public required string Payload { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime? ProcessedAt { get; set; }

    public string? ErrorMessage { get; set; }
}
