using AutoMapper;
using CoreBanking.Domain.Core.QueryDatabaseDtos;
using CoreBanking.Infrastructure.Core.Repos.Postgres.Entities;

namespace CoreBanking.Infrastructure.Core.Repos.Postgres.AutoMapper;

public class AutoMapperProfile : Profile
{
    public AutoMapperProfile()
    {
        CreateMap<CustomerEntity, CustomerDetails>().ReverseMap();
    }
}