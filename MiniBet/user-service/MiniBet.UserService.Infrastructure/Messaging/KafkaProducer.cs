using System;
using System.Text.Json;
using Confluent.Kafka;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MiniBet.UserService.Domain.Events;

namespace MiniBet.UserService.Infrastructure.Messaging;

public class KafkaProducer : IEventProducer, IDisposable
{
    private readonly IProducer<string, string> _producer;

    private readonly KafkaSettings _settings;

    private readonly ILogger<KafkaProducer> _logger;

    private bool _disposed;

    public KafkaProducer(IOptions<KafkaSettings> settings, ILogger<KafkaProducer> logger)
    {
        _settings = settings.Value;
        _logger = logger;

        var config = new ProducerConfig
        {
            BootstrapServers = _settings.BootstrapServers,
            Acks = Acks.All,
            EnableIdempotence = true,
            MessageSendMaxRetries = 3,
            RetryBackoffMs = 1000,
        };

        _producer = new ProducerBuilder<string, string>(config).Build();
    }

    public async Task ProduceAsync<T>(T @event) where T : UserEvent
    {
        if (_disposed)
        {
            throw new ObjectDisposedException(nameof(KafkaProducer));
        }

        var topic = GetTopicForEvent(@event);
        var json = JsonSerializer.Serialize(@event);

        try
        {
            var msg = new Message<string, string>
            {
                Key = @event.UserId.ToString(), // todo: check if this id must be correct.
                Value = json,
            };

            var deliveryResult = await _producer.ProduceAsync(topic, msg);
            _logger.LogInformation("Event {EventType} delivered to {Topic} [Partition: {Partition}, Offset: {Offset}]", typeof(T).Name, deliveryResult.Topic, deliveryResult.Partition, deliveryResult.Offset);
        }
        catch (ProduceException<string, string> ex)
        {
            _logger.LogError(ex, "Failed to deliver event {EventType} to {Topic}: {ErrorReason}", typeof(T).Name, topic, ex.Error.Reason);
            throw;
        }
    }

    private string GetTopicForEvent<T>(T @event) where T : UserEvent
    {
        return @event.GetType().Name.Replace("Event", string.Empty).ToLowerInvariant();
    }

    public void Dispose()
    {
        if (!_disposed)
        {
            _producer?.Dispose();
            _disposed = true;

            GC.SuppressFinalize(this);
        }
    }
}
