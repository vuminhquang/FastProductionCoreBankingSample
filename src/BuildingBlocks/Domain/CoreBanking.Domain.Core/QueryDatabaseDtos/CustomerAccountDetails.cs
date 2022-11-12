using CoreBanking.Domain.Core.Models;

namespace CoreBanking.Domain.Core.QueryDatabaseDtos;

public record CustomerAccountDetails(Guid Id, Money Balance)
{
    public static CustomerAccountDetails Map(Account account) 
        => new CustomerAccountDetails(account.Id, account.Balance);
}
