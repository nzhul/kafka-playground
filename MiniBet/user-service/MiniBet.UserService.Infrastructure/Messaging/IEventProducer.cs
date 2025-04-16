using System;
using MiniBet.UserService.Domain.Events;

namespace MiniBet.UserService.Infrastructure.Messaging;

public interface IEventProducer
{
    Task ProduceAsync<T>(T @event) where T : UserEvent;
}
