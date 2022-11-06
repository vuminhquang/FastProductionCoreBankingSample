using RepositoryHelper.Abstraction;

namespace CoreBanking.Infrastructure.Core.Repos.Postgres.Entities;

public class CustomerEntity : BaseEntity<Guid>
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Email { get; set; }
}