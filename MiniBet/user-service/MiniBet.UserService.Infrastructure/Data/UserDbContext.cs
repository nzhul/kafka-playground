using System.Reflection;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using MiniBet.UserService.Domain.Events;
using MiniBet.UserService.Infrastructure.Data.Entities;

namespace MiniBet.UserService.Infrastructure.Data;

public class UserDbContext : DbContext
{
    public DbSet<User> Users { get; set; }

    public DbSet<OutboxMessage> OutboxMessages { get; set; }

    public void AddEvent<T>(T @event) where T : UserEvent
    {
        var outboxMessage = new OutboxMessage
        {
            Id = Guid.NewGuid(),
            EventType = @event.GetType().Name,
            Payload = JsonSerializer.Serialize(@event),
            CreatedAt = DateTime.UtcNow
        };

        OutboxMessages.Add(outboxMessage);
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        base.OnModelCreating(builder);
    }
}
