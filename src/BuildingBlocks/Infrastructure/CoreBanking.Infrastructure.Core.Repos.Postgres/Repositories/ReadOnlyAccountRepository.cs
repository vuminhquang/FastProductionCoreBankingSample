using CoreBanking.Infrastructure.Core.Repos.Postgres.Entities;
using FreeBot.Domain.Repositories.Interfaces;
using RepositoryHelper;

namespace CoreBanking.Infrastructure.Core.Repos.Postgres.Repositories;

public class ReadOnlyAccountRepository : ReadOnlyGenericRepository<AccountEntity, Guid>
{
    public ReadOnlyAccountRepository(IUnitOfWork context) : base(context)
    {
    }
}