using CoreBanking.Infrastructure.Core.Repos.Postgres.Entities;
using FreeBot.Domain.Repositories.Interfaces;
using RepositoryHelper;

namespace CoreBanking.Infrastructure.Core.Repos.Postgres.Repositories;

public class ReadOnlyCustomerRepository : ReadOnlyGenericRepository<CustomerEntity, Guid>
{
    public ReadOnlyCustomerRepository(IUnitOfWork context) : base(context)
    {
    }
}