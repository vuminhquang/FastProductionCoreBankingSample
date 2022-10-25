using EventSourcing.EventBus;
using MediatR;

namespace CoreBanking.Domain.Core.IntegrationEvents
{
    public record AccountCreatedEvent : IIntegrationEvent, INotification
    {
        public AccountCreatedEvent(Guid id, Guid accountId)
        {
            this.Id = id;
            this.AccountId = accountId;
        }

        public Guid AccountId { get; init; }
        public Guid Id { get; }
    }
}