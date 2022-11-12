using AutoMapper;
using CoreBanking.Domain.Core.Models;
using CoreBanking.Domain.Core.QueryDatabaseDtos;
using CoreBanking.Infrastructure.Core.Repos.Postgres.Entities;

namespace CoreBanking.Infrastructure.Core.Repos.Postgres.AutoMapper;

public class AutoMapperProfile : Profile
{
    public AutoMapperProfile()
    {
        CreateMap<AccountEntity, AccountDetails>()
            .ConstructUsing(entity => new AccountDetails(entity.Id, entity.OwnerId,
                entity.Owner!.FirstName ?? string.Empty, entity.Owner.LastName ?? string.Empty, entity.Owner.Email,
                new Money(Currency.USDollar, entity.Balance)))
            .ReverseMap();

        CreateMap<CustomerEntity, CustomerDetails>()
            .ConstructUsing(entity => new CustomerDetails(entity.Id, entity.FirstName ?? string.Empty,
                entity.LastName ?? string.Empty, entity.Email,
                AutoMapperProfile.AccListConvert(entity.Accounts), new Money(Currency.USDollar, entity.Balance ?? 0)))
            .ReverseMap();
    }

    static CustomerAccountDetails[] AccListConvert(IList<AccountEntity> lst)
    {
        return lst.Select(entity => new CustomerAccountDetails(entity.Id, new Money(Currency.USDollar, entity.Balance))).ToArray();
    }
}