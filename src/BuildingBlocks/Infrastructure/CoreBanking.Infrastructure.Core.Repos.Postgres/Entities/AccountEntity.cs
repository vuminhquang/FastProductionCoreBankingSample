using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using RepositoryHelper.Abstraction;

namespace CoreBanking.Infrastructure.Core.Repos.Postgres.Entities;

public class AccountEntity : BaseEntity<Guid>
{
    [ForeignKey(nameof(CustomerEntity))]
    public Guid OwnerId { get; set; }
    public decimal Balance { get; set; }
    public string? BalanceCurrency { get; set; }
    
    [JsonIgnore] public virtual CustomerEntity? Owner { get; set; }
}