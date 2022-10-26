using CoreBanking.Application.Core.Models;
using MediatR;

namespace CoreBanking.Application.Core.Queries
{
    public record CustomerById(Guid CustomerId) : IRequest<CustomerDetails>;
}