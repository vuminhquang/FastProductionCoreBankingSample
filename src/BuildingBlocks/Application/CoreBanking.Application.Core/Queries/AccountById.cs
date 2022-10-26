using CoreBanking.Application.Core.Models;
using MediatR;

namespace CoreBanking.Application.Core.Queries
{
    public record AccountById(Guid AccountId) : IRequest<AccountDetails>;
}