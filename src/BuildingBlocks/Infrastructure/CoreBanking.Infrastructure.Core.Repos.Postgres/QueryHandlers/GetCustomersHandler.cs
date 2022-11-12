using AutoMapper;
using CoreBanking.Domain.Core.Queries;
using CoreBanking.Domain.Core.QueryDatabaseDtos;
using CoreBanking.Infrastructure.Core.Repos.Postgres.Entities;
using CoreBanking.Infrastructure.Core.Repos.Postgres.Repositories;
using MediatR;
using RepositoryHelper.Abstraction.Pagination;

namespace CoreBanking.Infrastructure.Core.Repos.Postgres.QueryHandlers;

public class GetCustomersHandler : IRequestHandler<GetCustomers, IEnumerable<CustomerDetails>>
{
    private readonly IMapper _mapper;
    private readonly ReadOnlyCustomerRepository _customerRepository;

    public GetCustomersHandler(IMapper mapper, ReadOnlyCustomerRepository customerRepository)
    {
        _mapper = mapper;
        _customerRepository = customerRepository;
    }

    public async Task<IEnumerable<CustomerDetails>> Handle(GetCustomers request, CancellationToken cancellationToken)
    {
        var ret = await _customerRepository.QueryHelper()
            // .Include(basket => basket.Articles)
            .GetAllAsync()
            ;
        return ret.Select(MapEntityToDto);
        // return _mapper.Map<IEnumerable<CustomerDetails>>(ret);
    }

    private CustomerDetails MapEntityToDto(CustomerEntity entity)
    {
        return _mapper.Map<CustomerDetails>(entity);
    }
}