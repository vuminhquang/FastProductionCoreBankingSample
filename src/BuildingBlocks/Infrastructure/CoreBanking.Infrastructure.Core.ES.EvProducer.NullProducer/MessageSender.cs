using EventSourcing.EventBus;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MultiThreadsEngine;

namespace CoreBanking.Infrastructure.Core.ES.EvProducer.NullProducer;

public class MessageSender : MultiThreadsTaskBase<IIntegrationEvent>
{
    private readonly IMediator _mediator;
    private readonly IServiceScopeFactory _serviceScopeFactory;
    private readonly IConfiguration _configuration;

    public MessageSender(IMediator mediator, IServiceScopeFactory serviceScopeFactory, IConfiguration configuration,
        int maxThreads = 1) : base(maxThreads)
    {
        _mediator = mediator;
        _serviceScopeFactory = serviceScopeFactory;
        _configuration = configuration;
    }

    protected override async Task ExecuteTask(IIntegrationEvent @event, int? taskNum, CancellationToken stoppingToken)
    {
        await _mediator.Publish(@event, stoppingToken);
    }
    
}