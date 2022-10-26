using CoreBanking.Domain.Core.Commands;
using CoreBanking.Domain.Core.IntegrationEvents;
using CoreBanking.Domain.Core.Models;
using CoreBanking.Domain.Core.Services;
using EventSourcing;
using EventSourcing.EventBus;
using MediatR;

namespace CoreBanking.Infrastructure.Core.CommandHandlers;

public class DepositHandler : INotificationHandler<Deposit>
{
    private readonly IAggregateRepository<Account, Guid> _accountEventsService;
    private readonly ICurrencyConverter _currencyConverter;
    private readonly IEventProducer _eventProducer;

    public DepositHandler(IAggregateRepository<Account, Guid> accountEventsService, ICurrencyConverter currencyConverter, IEventProducer eventProducer)
    {
        _accountEventsService = accountEventsService;
        _currencyConverter = currencyConverter;
        _eventProducer = eventProducer;
    }

    public async Task Handle(Deposit command, CancellationToken cancellationToken)
    {
        var account = await _accountEventsService.RehydrateAsync(command.AccountId, cancellationToken);
        if(null == account)
            throw new ArgumentOutOfRangeException(nameof(Deposit.AccountId), "invalid account id");

        account.Deposit(command.Amount, _currencyConverter);

        await _accountEventsService.PersistAsync(account, cancellationToken);

        var @event = new TransactionHappenedEvent(Guid.NewGuid(), account.Id);
        await _eventProducer.DispatchAsync(@event, cancellationToken);
    }
}