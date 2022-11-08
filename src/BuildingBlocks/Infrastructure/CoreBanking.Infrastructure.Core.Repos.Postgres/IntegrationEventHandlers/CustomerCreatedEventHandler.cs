using CoreBanking.Domain.Core.IntegrationEvents;
using CoreBanking.Domain.Core.Models;
using CoreBanking.Domain.Core.Services;
using CoreBanking.Domain.Core.SummarizationDtos;
using CoreBanking.Infrastructure.Core.Repos.Postgres.Entities;
using CoreBanking.Infrastructure.Core.Repos.Postgres.Repositories;
using EventSourcing;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace CoreBanking.Infrastructure.Core.Repos.Postgres.IntegrationEventHandlers;

public class CustomerCreatedEventHandler : INotificationHandler<CustomerCreatedEvent>
{
    private readonly IAggregateRepository<Customer, Guid> _customersEventService;
    private readonly IAggregateRepository<Account, Guid> _accountsEventService;
    private readonly IServiceScopeFactory _serviceScopeFactory;

    // private readonly CustomerRepository _customerDbRepository;
    private readonly ICurrencyConverter _currencyConverter;
    private readonly ILogger<CustomerCreatedEventHandler> _logger;

    public CustomerCreatedEventHandler(IAggregateRepository<Customer, Guid> customersEventService,
        IAggregateRepository<Account, Guid> accountsEventService,
        IServiceScopeFactory serviceScopeFactory,
        // CustomerRepository customerDbRepository,
        ICurrencyConverter currencyConverter,
        ILogger<CustomerCreatedEventHandler> logger)
    {
        // _customerDbRepository = customerDbRepository;
        _currencyConverter = currencyConverter;
        _logger = logger;
        _accountsEventService = accountsEventService;
        _serviceScopeFactory = serviceScopeFactory;
        _customersEventService = customersEventService;
    }

    public async Task Handle(CustomerCreatedEvent @event, CancellationToken cancellationToken)
    {
        _logger.LogInformation("creating customer details for customer {CustomerId} ...", @event.CustomerId);

        // Get fresh details from events
        var customerView = await SummarizeDataViaEvents(@event.CustomerId, cancellationToken);
        
        // Save to QueryDatabase
        await SaveCustomerAsync(customerView, cancellationToken);
        
    }

    protected async Task SaveCustomerAsync(CustomerDetails customerView, CancellationToken cancellationToken)
    {
        var customer = new CustomerEntity
        {
            Id = customerView.Id,
            FirstName = customerView.Firstname,
            LastName = customerView.Lastname,
            Email = customerView.Email,
            Balance = customerView.TotalBalance.Value,
            BalanceCurrency = customerView.TotalBalance.Currency.Symbol
        };
        // Avoid: The instance of entity type cannot be tracked because another instance with the same key value
        // is already being tracked.
        await using var scope = _serviceScopeFactory.CreateAsyncScope();
        var custRepo = scope.ServiceProvider.GetRequiredService<CustomerRepository>();
        custRepo.Update(customer);
        await custRepo.SaveChangesAsync(cancellationToken);
        // _customerDbRepository.Update(customer);
        // await _customerDbRepository.SaveChangesAsync(cancellationToken);
    }

    protected async Task<CustomerDetails> SummarizeDataViaEvents(Guid customerId, CancellationToken cancellationToken)
    {
        var customer = await _customersEventService.RehydrateAsync(customerId, cancellationToken);

        var totalBalance = Money.Zero(Currency.USDollar);
        var accounts = new CustomerAccountDetails[customer.Accounts.Count];
        int index = 0;
        foreach (var id in customer.Accounts)
        {
            var account = await _accountsEventService.RehydrateAsync(id, cancellationToken);
            accounts[index++] = CustomerAccountDetails.Map(account);

            totalBalance = totalBalance.Add(account.Balance, _currencyConverter);
        }

        var customerView = new CustomerDetails(customer.Id, customer.Firstname, customer.Lastname, customer.Email.Value, accounts, totalBalance);
        return customerView;
    }
}