using EventSourcing.EventBus;
using MediatR;

namespace CoreBanking.Domain.Core.IntegrationEvents
{
    public record TransactionHappenedEvent : IIntegrationEvent, INotification
    {
        public TransactionHappenedEvent(Guid id, Guid accountId)
        {
            this.Id = id;
            this.AccountId = accountId;
        }

        public Guid AccountId { get; init; }
        public Guid Id { get; }
    }
}