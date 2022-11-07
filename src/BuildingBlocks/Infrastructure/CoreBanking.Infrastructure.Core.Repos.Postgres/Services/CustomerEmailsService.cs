using CoreBanking.Domain.Core.Services;
using CoreBanking.Infrastructure.Core.Repos.Postgres.Entities;
using CoreBanking.Infrastructure.Core.Repos.Postgres.Repositories;

namespace CoreBanking.Infrastructure.Core.Repos.Postgres.Services;

public class CustomerEmailsService :  ICustomerEmailsService
{
    private readonly ReadOnlyCustomerRepository _roCustomerRepository;
    private readonly CustomerRepository _customerRepository;

    public CustomerEmailsService(ReadOnlyCustomerRepository roCustomerRepository, CustomerRepository customerRepository)
    {
        _roCustomerRepository = roCustomerRepository;
        _customerRepository = customerRepository;
    }

    public async Task<bool> ExistsAsync(string email)
    {
        var entity = await _roCustomerRepository.QueryHelper()
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

        _customerRepository.Add(customer);
        await _customerRepository.SaveChangesAsync();
    }
}