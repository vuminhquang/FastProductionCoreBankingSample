using AutoMapper;
using CoreBanking.Domain.Core.Queries;
using CoreBanking.Domain.Core.QueryDatabaseDtos;
using CoreBanking.Infrastructure.Core.Repos.Postgres.Repositories;
using MediatR;

namespace CoreBanking.Infrastructure.Core.Repos.Postgres.QueryHandlers;

public class GetCustomerByIdHandler : IRequestHandler<GetCustomerById, CustomerDetails>
{
    private readonly IMapper _mapper;
    private readonly ReadOnlyCustomerRepository _customerRepository;

    public GetCustomerByIdHandler(IMapper mapper, ReadOnlyCustomerRepository customerRepository)
    {
        _mapper = mapper;
        _customerRepository = customerRepository;
    }

    public async Task<CustomerDetails> Handle(GetCustomerById request, CancellationToken cancellationToken)
    {
        var ret = await _customerRepository.QueryHelper()
            .GetOneAsync(entity => entity.Id == request.CustomerId);
        return _mapper.Map<CustomerDetails>(ret);
    }
}