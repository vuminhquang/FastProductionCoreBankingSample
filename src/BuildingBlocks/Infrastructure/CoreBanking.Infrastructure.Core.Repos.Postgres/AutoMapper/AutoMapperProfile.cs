using AutoMapper;
using CoreBanking.Domain.Core.Models;
using CoreBanking.Domain.Core.QueryDatabaseDtos;
using CoreBanking.Infrastructure.Core.Repos.Postgres.Entities;

namespace CoreBanking.Infrastructure.Core.Repos.Postgres.AutoMapper;

public class AutoMapperProfile : Profile
{
    public AutoMapperProfile()
    {
        CreateMap<CustomerEntity, CustomerDetails>()
            .ConstructUsing(entity => new CustomerDetails(entity.Id, entity.FirstName ?? string.Empty,
                entity.LastName ?? string.Empty, entity.Email,
                AutoMapperProfile.AccListConvert(entity.Accounts), new Money(Currency.FromCode(entity.BalanceCurrency ?? Currency.USDollar.Symbol), entity.Balance ?? 0)))
            .ForMember(dst => dst.Accounts, conf =>
            {
                conf.MapFrom(entity => AccListConvert(entity.Accounts));
            })
            .ReverseMap();
        
        CreateMap<AccountEntity, AccountDetails>()
            .ConstructUsing(entity => new AccountDetails(entity.Id, entity.OwnerId,
                entity.Owner!.FirstName ?? string.Empty, entity.Owner.LastName ?? string.Empty, entity.Owner.Email,
                new Money(Currency.FromCode(entity.BalanceCurrency ?? Currency.USDollar.Symbol), entity.Balance)))
            .ReverseMap();
    }

    static CustomerAccountDetails[] AccListConvert(IList<AccountEntity> lst)
    {
        return lst.Select(entity => new CustomerAccountDetails(entity.Id, new Money(Currency.FromCode(entity.BalanceCurrency ?? Currency.USDollar.Symbol), entity.Balance))).ToArray();
    }
}