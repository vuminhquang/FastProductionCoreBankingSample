using CoreBanking.Domain.Core.Models;
using CoreBanking.Infrastructure.Core.Repos.Postgres.Entities;
using FreeBot.Domain.Repositories.Interfaces;
using RepositoryHelper;

namespace CoreBanking.Infrastructure.Core.Repos.Postgres;

public class CustomerRepository : GenericRepository<CustomerEntity,Guid>
{
    public CustomerRepository(IUnitOfWork context) : base(context)
    {
    }
}