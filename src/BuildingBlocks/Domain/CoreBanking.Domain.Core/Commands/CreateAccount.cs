using CoreBanking.Domain.Core.Models;
using MediatR;

namespace CoreBanking.Domain.Core.Commands
{
    public record CreateAccount : INotification
    {
        public CreateAccount(Guid customerId, Guid accountId, Currency currency)
        {
            CustomerId = customerId;
            AccountId = accountId;
            Currency = currency ?? throw new ArgumentNullException(nameof(currency));
        }

        public Guid CustomerId { get; }
        public Guid AccountId { get; }
        public Currency Currency { get; }
    }
}