using CoreBanking.Domain.Core.IntegrationEvents;
using CoreBanking.Domain.Core.Models;
using CoreBanking.Domain.Core.QueryDatabaseDtos;
using CoreBanking.Infrastructure.Core.Repos.Postgres.Entities;
using CoreBanking.Infrastructure.Core.Repos.Postgres.Repositories;
using EventSourcing;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace CoreBanking.Infrastructure.Core.Repos.Postgres.IntegrationEventHandlers;

public class AccountEventsHandlers : INotificationHandler<AccountCreatedEvent>,
    INotificationHandler<TransactionCreatedEvent>
{
    private readonly IAggregateRepository<Customer, Guid> _customersEventService;
    private readonly IAggregateRepository<Account, Guid> _accountsEventService;
    private readonly IServiceScopeFactory _serviceScopeFactory;
    private readonly ILogger<AccountEventsHandlers> _logger;

    public AccountEventsHandlers(
        IAggregateRepository<Customer, Guid> customersEventService,
        IAggregateRepository<Account, Guid> accountsEventService,
        IServiceScopeFactory serviceScopeFactory,
        ILogger<AccountEventsHandlers> logger)
    {
        _customersEventService = customersEventService;
        _accountsEventService = accountsEventService;
        _serviceScopeFactory = serviceScopeFactory;
        _logger = logger;
    }

    public async Task Handle(AccountCreatedEvent @event, CancellationToken cancellationToken)
    {
        _logger.LogInformation("updating details for account {AccountId} ...", @event.AccountId);

        var accountView = await CalculateBasedOnEvents(@event.AccountId, cancellationToken);
        await SaveAccountAsync(accountView, cancellationToken);
    }
    
    public async Task Handle(TransactionCreatedEvent @event, CancellationToken cancellationToken)
    {
        _logger.LogInformation("processing transaction on account {AccountId} ...", @event.AccountId);

        var accountView = await CalculateBasedOnEvents(@event.AccountId, cancellationToken);
        await SaveAccountAsync(accountView, cancellationToken);
    }

    private async Task<AccountDetails> CalculateBasedOnEvents(Guid accountId, CancellationToken cancellationToken)
    {
        var account = await _accountsEventService.RehydrateAsync(accountId, cancellationToken);
        var customer = await _customersEventService.RehydrateAsync(account.OwnerId, cancellationToken);

        var accountView = new AccountDetails(account.Id,
            account.OwnerId, customer.Firstname, customer.Lastname, customer.Email.Value,
            account.Balance);
        return accountView;
    }

    private async Task SaveAccountAsync(AccountDetails accountView, CancellationToken cancellationToken)
    {
        var account = new AccountEntity
        {
            OwnerId = accountView.OwnerId,
            Balance = accountView.Balance.Value,
            BalanceCurrency = accountView.Balance.Currency.Symbol
        };
        
        await using var scope = _serviceScopeFactory.CreateAsyncScope();
        var accountRepository = scope.ServiceProvider.GetRequiredService<AccountRepository>();
        accountRepository.Add(account);
        await accountRepository.SaveChangesAsync(cancellationToken);
    }
}
