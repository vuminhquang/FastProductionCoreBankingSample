using MediatR;

namespace CoreBanking.Domain.Core.Commands
{
    public record CreateCustomer(Guid CustomerId, string FirstName, string LastName, string Email) : INotification;
}