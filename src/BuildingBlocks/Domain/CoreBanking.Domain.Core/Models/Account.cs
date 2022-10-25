using CoreBanking.Domain.Core.DomainEvents;
using CoreBanking.Domain.Core.Services;
using EventSourcing.Models;
using OrderingServer.Domain.EventSourcing.Abstractions.Models;
using SuperSafeBank.Domain;

namespace CoreBanking.Domain.Core.Models
{
    public record Account : BaseAggregateRoot<Account, Guid>
    {
        private Account() { }

        public Account(Guid id, Customer owner, Currency currency) : base(id)
        {
            if (owner == null) 
                throw new ArgumentNullException(nameof(owner));
            if (currency == null)
                throw new ArgumentNullException(nameof(currency));
                        
            this.Append(new AccCreatedDomainEvent(this, owner, currency));
        }

        public Guid OwnerId { get; private set; }
        public Money Balance { get; private set; }

        public void Withdraw(Money amount, ICurrencyConverter currencyConverter)
        {
            if (amount.Value < 0)
                throw new ArgumentOutOfRangeException(nameof(amount),"amount cannot be negative");

            var normalizedAmount = currencyConverter.Convert(amount, this.Balance.Currency);
            if (normalizedAmount.Value > this.Balance.Value)
                throw new AccountTransactionException($"unable to withdrawn {normalizedAmount} from account {this.Id}", this);

            this.Append(new AccWithdrawalDomainEvent(this, amount));
        }

        public void Deposit(Money amount, ICurrencyConverter currencyConverter)
        {
            if(amount.Value < 0)
                throw new ArgumentOutOfRangeException(nameof(amount), "amount cannot be negative");
            
            var normalizedAmount = currencyConverter.Convert(amount, this.Balance.Currency);
            
            this.Append(new AccDepositDomainEvent(this, normalizedAmount));
        }

        protected override void When(IDomainEvent<Guid> @event)
        {
            switch (@event)
            {
                case AccCreatedDomainEvent c:
                    this.Id = c.AggregateId;
                    this.Balance = Money.Zero(c.Currency);
                    this.OwnerId = c.OwnerId;
                    break;
                case AccWithdrawalDomainEvent w:
                    this.Balance = this.Balance.Subtract(w.Amount);
                    break;
                case AccDepositDomainEvent d:
                    this.Balance = this.Balance.Add(d.Amount);
                    break;
            }
        }

        public static Account Create(Guid accountId, Customer owner, Currency currency)
        {
            var account = new Account(accountId, owner, currency);
            owner.AddAccount(account);
            return account;
        }
    }
}