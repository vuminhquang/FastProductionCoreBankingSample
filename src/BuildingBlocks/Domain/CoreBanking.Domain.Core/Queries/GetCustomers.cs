using CoreBanking.Domain.Core.QueryDatabaseDtos;
using MediatR;

namespace CoreBanking.Domain.Core.Queries;

public record GetCustomers() : IRequest<IEnumerable<CustomerDetails>>;