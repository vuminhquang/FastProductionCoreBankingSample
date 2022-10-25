using EventSourcing.EventBus;
using MediatR;

namespace CoreBanking.Domain.Core.IntegrationEvents
{
    public record CustomerCreatedEvent : IIntegrationEvent, INotification
    {
        public CustomerCreatedEvent(Guid id, Guid customerId)
        {
            this.Id = id;
            this.CustomerId = customerId;
        }

        public Guid Id { get; }
        public Guid CustomerId { get; }
    }
}