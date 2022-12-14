using System.ComponentModel.DataAnnotations.Schema;
using RepositoryHelper.Abstraction;

namespace CoreBanking.Infrastructure.Core.Repos.Postgres.Entities;

[Table("customer")]
public class CustomerEntity : BaseEntity<Guid>
{
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string Email { get; set; }
    public decimal? Balance { get; set; }
    public string? BalanceCurrency { get; set; }
    
    public IList<AccountEntity> Accounts { get; set; } = new List<AccountEntity>();
}