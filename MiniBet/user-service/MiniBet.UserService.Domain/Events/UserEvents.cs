namespace MiniBet.UserService.Domain.Events;

using System;

public abstract record UserEvent
{
    public required string UserId { get; set; }

    public required DateTime Timestamp { get; set; }
}
