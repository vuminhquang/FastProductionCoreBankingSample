using CoreBanking.Domain.Core.Commands;
using CoreBanking.Domain.Core.IntegrationEvents;
using CoreBanking.Domain.Core.Models;
using EventSourcing;
using EventSourcing.EventBus;
using MediatR;

namespace CoreBanking.Infrastructure.Core.CommandHandlers;

public class CreateAccountHandler : INotificationHandler<CreateAccount>
{
    private readonly IAggregateRepository<Customer, Guid> _customerEventsService;
    private readonly IAggregateRepository<Account, Guid> _accountEventsService;
    private readonly IEventProducer _eventProducer;

    public CreateAccountHandler(IAggregateRepository<Customer, Guid> customerEventsService, IAggregateRepository<Account, Guid> accountEventsService, IEventProducer eventProducer)
    {
        _customerEventsService = customerEventsService;
        _accountEventsService = accountEventsService;
        _eventProducer = eventProducer;
    }

    public async Task Handle(CreateAccount command, CancellationToken cancellationToken)
    {
        var customer = await _customerEventsService.RehydrateAsync(command.CustomerId, cancellationToken);
        if(null == customer)
            throw new ArgumentOutOfRangeException(nameof(CreateAccount.CustomerId), "invalid customer id");

        var account = Account.Create(command.AccountId, customer, command.Currency);

        await _customerEventsService.PersistAsync(customer, cancellationToken);
        await _accountEventsService.PersistAsync(account, cancellationToken);

        var @event = new AccountCreatedEvent(Guid.NewGuid(), account.Id);
        await _eventProducer.DispatchAsync(@event, cancellationToken);
    }
}