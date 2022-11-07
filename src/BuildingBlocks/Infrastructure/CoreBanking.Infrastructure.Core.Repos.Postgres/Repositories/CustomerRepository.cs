using CoreBanking.Domain.Core.Services;
using CoreBanking.Infrastructure.Core.Repos.Postgres.Entities;
using FreeBot.Domain.Repositories.Interfaces;
using RepositoryHelper;

namespace CoreBanking.Infrastructure.Core.Repos.Postgres.Repositories;

public class CustomerRepository : GenericRepository<CustomerEntity, Guid>
{
    public CustomerRepository(IUnitOfWork context) : base(context)
    {
    }
}