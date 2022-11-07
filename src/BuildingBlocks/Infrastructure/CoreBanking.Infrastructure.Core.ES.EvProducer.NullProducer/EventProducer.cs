using EventSourcing.EventBus;
using Microsoft.Extensions.Logging;

namespace CoreBanking.Infrastructure.Core.ES.EvProducer.NullProducer;

public class EventProducer : IEventProducer
{
    private readonly ILogger<EventProducer> _logger;

    public EventProducer(ILogger<EventProducer> logger)
    {
        _logger = logger;
    }

    public Task DispatchAsync(IIntegrationEvent @event, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Sent event to message queue {Event}", @event);
        return Task.CompletedTask;
    }
}