using System;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MiniBet.UserService.Domain.Events;
using MiniBet.UserService.Infrastructure.Data;
using MiniBet.UserService.Infrastructure.Messaging;

namespace MiniBet.UserService.Infrastructure.BackgroundServices;

public class OutboxProcessorService : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<OutboxProcessorService> _logger;
    private readonly TimeSpan _processInterval = TimeSpan.FromSeconds(10); // Adjust the interval as needed

    // todo: use primary constructor
    public OutboxProcessorService(IServiceScopeFactory scopeFactory, ILogger<OutboxProcessorService> logger)
    {
        _scopeFactory = scopeFactory;
        _logger = logger;
    }


    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            await ProcessOutboxMessages(stoppingToken);
            await Task.Delay(_processInterval, stoppingToken);
        }
    }

    private async Task ProcessOutboxMessages(CancellationToken stoppingToken)
    {
        try
        {
            using var scope = _scopeFactory.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<UserDbContext>();
            var eventPublisher = scope.ServiceProvider.GetRequiredKeyedService<IEventPublisher>();

            var messages = await dbContext.OutboxMessages
                .Where(m => m.ProcessedAt == null)
                .OrderBy(m => m.CreatedAt)
                .Take(20)
                .ToListAsync(stoppingToken);

            foreach (var message in messages)
            {
                // todo: refactor this validation

                if (string.IsNullOrEmpty(message.EventType))
                {
                    message.ErrorMessage = "Event type is null or empty";
                    message.ProcessedAt = DateTime.UtcNow;
                    continue;
                }

                var eventType = Type.GetType(message.EventType);
                if (eventType == null)
                {
                    message.ErrorMessage = $"Could not resolve event type '{message.EventType}'";
                    message.ProcessedAt = DateTime.UtcNow;
                    continue;
                }

                var domainEvent = JsonSerializer.Deserialize(message.Payload, eventType);
                if (domainEvent == null)
                {
                    message.ErrorMessage = "Could not deserialize event data";
                    message.ProcessedAt = DateTime.UtcNow;
                    continue;
                }

                await eventPublisher

                var produceMethod = typeof(IEventProducer).GetMethod("ProduceAsync");
                var genericMethod = produceMethod!.MakeGenericMethod(eventType);
            }
        }
        catch (System.Exception ex)
        {

            throw;
        }
    }
}
