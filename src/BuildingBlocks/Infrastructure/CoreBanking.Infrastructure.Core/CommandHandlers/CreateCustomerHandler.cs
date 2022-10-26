using CoreBanking.Domain.Core.Commands;
using CoreBanking.Domain.Core.IntegrationEvents;
using CoreBanking.Domain.Core.Models;
using CoreBanking.Domain.Core.Services;
using EventSourcing;
using EventSourcing.EventBus;
using MediatR;
using OrderingServer.Domain.EventSourcing.Abstractions;

namespace CoreBanking.Infrastructure.Core.CommandHandlers;

public class CreateCustomerHandler : INotificationHandler<CreateCustomer>
{
    private readonly IAggregateRepository<Customer, Guid> _eventsService;
    private readonly ICustomerEmailsService _customerEmailsRepository;
    private readonly IEventProducer _eventProducer;

    public CreateCustomerHandler(
        IAggregateRepository<Customer, Guid> eventsService, 
        ICustomerEmailsService customerEmailsRepository, 
        IEventProducer eventProducer)
    {
        _eventsService = eventsService ?? throw new ArgumentNullException(nameof(eventsService));
        _customerEmailsRepository = customerEmailsRepository ?? throw new ArgumentNullException(nameof(customerEmailsRepository));
        _eventProducer = eventProducer ?? throw new ArgumentNullException(nameof(eventProducer));
    }

    public async Task Handle(CreateCustomer command, CancellationToken cancellationToken)
    {
        if(string.IsNullOrWhiteSpace(command.Email))
            throw new ValidationException("Invalid email address", new ValidationError(nameof(CreateCustomer.Email), "email cannot be empty"));

        if (await _customerEmailsRepository.ExistsAsync(command.Email))
            throw new ValidationException("Duplicate email address", new ValidationError(nameof(CreateCustomer.Email), $"email '{command.Email}' already exists"));
            
        var customer = Customer.Create(command.CustomerId, command.FirstName, command.LastName, command.Email);
        await _eventsService.PersistAsync(customer, cancellationToken);
        await _customerEmailsRepository.CreateAsync(command.Email, customer.Id);

        var @event = new CustomerCreatedEvent(Guid.NewGuid(), command.CustomerId);
        await _eventProducer.DispatchAsync(@event, cancellationToken);
    }
}