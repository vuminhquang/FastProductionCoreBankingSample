using CoreBanking.Domain.Core.Models;
using OrderingServer.Domain.EventSourcing.Abstractions.Models;

namespace CoreBanking.Domain.Core.DomainEvents
{
    public record CustAccAddedDomainEvent : BaseDomainEvent<Customer, Guid>
    {
        private CustAccAddedDomainEvent() { }

        public CustAccAddedDomainEvent(Customer customer, Guid accountId) : base(customer)
        {
            AccountId = accountId;
        }

        public Guid AccountId { get; init; }
    }
}