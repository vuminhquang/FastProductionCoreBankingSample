using CoreBanking.Domain.Core.Models;

namespace CoreBanking.Application.Core.Models;

public record CustomerAccountDetails(Guid Id, Money Balance)
{
    public static CustomerAccountDetails Map(Account account) 
        => new CustomerAccountDetails(account.Id, account.Balance);
}