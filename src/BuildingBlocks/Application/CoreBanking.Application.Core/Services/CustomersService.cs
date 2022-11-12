using CoreBanking.Domain.Core.Queries;
using CoreBanking.Domain.Core.QueryDatabaseDtos;
using MediatR;

namespace CoreBanking.Application.Core.Services;

public class CustomersService
{
    private readonly IMediator _mediator;

    public CustomersService(IMediator mediator)
    {
        _mediator = mediator;
    }

    public async Task<CustomerDetails> GetCustomer(Guid id, CancellationToken cancellationToken = default)
    {
        return null;
    }    
    
    public async Task<IEnumerable<CustomerDetails>> GetCustomers(CancellationToken cancellationToken = default)
    {
        var results = await _mediator.Send(new GetCustomers(), cancellationToken);
        return results;
    }
}