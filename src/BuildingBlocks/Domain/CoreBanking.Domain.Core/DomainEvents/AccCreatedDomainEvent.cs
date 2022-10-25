using CoreBanking.Domain.Core.Models;
using OrderingServer.Domain.EventSourcing.Abstractions.Models;
using SuperSafeBank.Domain;

namespace CoreBanking.Domain.Core.DomainEvents
{
    public record AccCreatedDomainEvent : BaseDomainEvent<Account, Guid>
    {
        /// <summary>
        /// for deserialization
        /// </summary>
        private AccCreatedDomainEvent() { }

        public AccCreatedDomainEvent(Account account, Customer owner, Currency currency) : base(account)
        {
            if (owner is null)
                throw new ArgumentNullException(nameof(owner));

            OwnerId = owner.Id;
            Currency = currency ?? throw new ArgumentNullException(nameof(currency));
        }

        public Guid OwnerId { get; init; }
        public Currency Currency { get; init; }
    }
}