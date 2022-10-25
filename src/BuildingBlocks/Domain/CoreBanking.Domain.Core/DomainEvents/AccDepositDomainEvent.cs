using CoreBanking.Domain.Core.Models;
using OrderingServer.Domain.EventSourcing.Abstractions.Models;
using SuperSafeBank.Domain;

namespace CoreBanking.Domain.Core.DomainEvents;

public record AccDepositDomainEvent : BaseDomainEvent<Account, Guid>
{
    /// <summary>
    /// for deserialization
    /// </summary>
    private AccDepositDomainEvent() { }

    public AccDepositDomainEvent(Account account, Money amount) : base(account)
    {
        Amount = amount;
    }

    public Money Amount { get; init; }
}