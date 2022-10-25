using CoreBanking.Domain.Core.DomainEvents;
using EventSourcing.Models;
using OrderingServer.Domain.EventSourcing.Abstractions.Models;
using SuperSafeBank.Domain;

namespace CoreBanking.Domain.Core.Models
{
    public record Customer : BaseAggregateRoot<Customer, Guid>
    {
        private readonly HashSet<Guid> _accounts = new();

        private Customer() { }
        
        public Customer(Guid id, string firstname, string lastname, Email email) : base(id)
        {
            if(string.IsNullOrWhiteSpace(firstname))
                throw new ArgumentNullException(nameof(firstname));
            if (string.IsNullOrWhiteSpace(lastname))
                throw new ArgumentNullException(nameof(lastname));
            if (email is null)            
                throw new ArgumentNullException(nameof(email));
            
            this.Append(new CustCreatedDomainEvent(this, firstname, lastname, email));
        }

        public void AddAccount(Account account)
        {
            if (account is null)
                throw new ArgumentNullException(nameof(account));
            
            if (_accounts.Contains(account.Id))
                return;

            this.Append(new CustAccAddedDomainEvent(this, account.Id));
        }

        public string Firstname { get; private set; }
        public string Lastname { get; private set; }
        public Email Email { get; private set; }
        public IReadOnlyCollection<Guid> Accounts => _accounts;

        protected override void When(IDomainEvent<Guid> @event)
        {
            switch (@event)
            {
                case CustCreatedDomainEvent c:
                    this.Id = c.AggregateId;
                    this.Firstname = c.Firstname;
                    this.Lastname = c.Lastname;
                    this.Email = c.Email;
                    break;
                case CustAccAddedDomainEvent aa:
                    _accounts.Add(aa.AccountId);
                    break;
            }
        }

        public static Customer Create(Guid customerId, string firstName, string lastName, string email)
        {
            return new Customer(customerId, firstName, lastName, new Email(email));
        }
    }
}