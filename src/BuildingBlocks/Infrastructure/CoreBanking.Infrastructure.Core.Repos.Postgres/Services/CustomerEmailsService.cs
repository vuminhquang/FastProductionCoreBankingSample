using CoreBanking.Domain.Core.Services;
using CoreBanking.Infrastructure.Core.Repos.Postgres.Entities;
using CoreBanking.Infrastructure.Core.Repos.Postgres.Repositories;
using Microsoft.Extensions.DependencyInjection;

namespace CoreBanking.Infrastructure.Core.Repos.Postgres.Services;

public class CustomerEmailsService :  ICustomerEmailsService
{
    private readonly IServiceScopeFactory _serviceScopeFactory;

    public CustomerEmailsService(IServiceScopeFactory serviceScopeFactory)
    {
        _serviceScopeFactory = serviceScopeFactory;
    }

    public async Task<bool> ExistsAsync(string email)
    {
        using var scope = _serviceScopeFactory.CreateScope();
        var custRepo = scope.ServiceProvider.GetRequiredService<ReadOnlyCustomerRepository>();
        var entity = await custRepo.QueryHelper()
            .GetOneAsync(customer => customer.Email.ToLower() == email.ToLower());
        return entity != null;
    }

    public async Task CreateAsync(string email, Guid customerId)
    {
        var customer = new CustomerEntity
        {
            Id = customerId,
            Email = email
        };

        using var scope = _serviceScopeFactory.CreateScope();
        var custRepo = scope.ServiceProvider.GetRequiredService<CustomerRepository>();
        custRepo.Add(customer);
        await custRepo.SaveChangesAsync();
    }
}