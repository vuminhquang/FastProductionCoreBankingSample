using EventSourcing.EventBus;
using MediatR;
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
        MessageQueueSimulator.RegisterWrapper.RegisterToRun(@event);
        // await _mediator.Publish(@event, cancellationToken);
        _logger.LogInformation("Sent event to message queue {Event}", @event);
        return Task.CompletedTask;
    }
}