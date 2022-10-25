using CoreBanking.Domain.Core.IntegrationEvents;
using CoreBanking.Domain.Core.Models;
using CoreBanking.Domain.Core.Services;
using EventSourcing;
using EventSourcing.EventBus;
using MediatR;

namespace CoreBanking.Domain.Core.Commands;

public class WithdrawHandler : INotificationHandler<Withdraw>
{
    private readonly IAggregateRepository<Account, Guid> _accountEventsService;
    private readonly ICurrencyConverter _currencyConverter;
    private readonly IEventProducer _eventProducer;

    public WithdrawHandler(IAggregateRepository<Account, Guid> accountEventsService, ICurrencyConverter currencyConverter, IEventProducer eventProducer)
    {
        _accountEventsService = accountEventsService;
        _currencyConverter = currencyConverter;
        _eventProducer = eventProducer;
    }

    public async Task Handle(Withdraw command, CancellationToken cancellationToken)
    {
        var account = await _accountEventsService.RehydrateAsync(command.AccountId);
        if (null == account)
            throw new ArgumentOutOfRangeException(nameof(Withdraw.AccountId), "invalid account id");

        account.Withdraw(command.Amount, _currencyConverter);

        await _accountEventsService.PersistAsync(account);

        var @event = new TransactionHappenedEvent(Guid.NewGuid(), account.Id);
        await _eventProducer.DispatchAsync(@event, cancellationToken);
    }
}