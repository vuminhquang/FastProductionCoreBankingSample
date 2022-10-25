using CoreBanking.Domain.Core.Models;
using MediatR;

namespace CoreBanking.Domain.Core.Commands
{
    public record Withdraw : INotification
    {
        public Withdraw(Guid accountId, Money amount)
        {
            AccountId = accountId;
            Amount = amount ?? throw new ArgumentNullException(nameof(amount));
        }

        public Guid AccountId { get; }
        public Money Amount { get; }
    }
}