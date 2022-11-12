using CoreBanking.Domain.Core.QueryDatabaseDtos;
using MediatR;

namespace CoreBanking.Domain.Core.Queries;

public record GetCustomerById(Guid CustomerId) : IRequest<CustomerDetails>;