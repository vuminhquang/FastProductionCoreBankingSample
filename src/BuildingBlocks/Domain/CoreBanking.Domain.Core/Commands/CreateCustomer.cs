using MediatR;

namespace CoreBanking.Domain.Core.Commands
{
    public record CreateCustomer : INotification
    {
        public CreateCustomer(Guid id, string firstName, string lastName, string email)
        {
            this.CustomerId = id;
            this.FirstName = firstName;
            this.LastName = lastName;
            this.Email = email;
        }

        public Guid CustomerId {get;}
        public string FirstName { get; }
        public string LastName { get; }
        public string Email { get; }
    }
}