using CoreBanking.Domain.Core.Models;
using OrderingServer.Domain.EventSourcing.Abstractions.Models;
using SuperSafeBank.Domain;

namespace CoreBanking.Domain.Core.DomainEvents;

public record CustCreatedDomainEvent : BaseDomainEvent<Customer, Guid>
{
    /// <summary>
    /// for deserialization
    /// </summary>
    private CustCreatedDomainEvent() { }

    public CustCreatedDomainEvent(Customer customer, string firstname, string lastname, Email email) : base(customer)
    {
        Firstname = firstname;
        Lastname = lastname;
        Email = email;
    }

    public string Firstname { get; init; }
    public string Lastname { get; init; }
    public Email Email { get; init; }
}