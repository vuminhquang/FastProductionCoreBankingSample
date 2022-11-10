using CoreBanking.Domain.Core.Services;
using CoreBanking.Infrastructure.Core.Repos.Postgres.Entities;
using FreeBot.Domain.Repositories.Interfaces;
using RepositoryHelper;

namespace CoreBanking.Infrastructure.Core.Repos.Postgres.Repositories;

public class AccountRepository : GenericRepository<AccountEntity, Guid>
{
    public AccountRepository(IUnitOfWork context) : base(context)
    {
    }
}