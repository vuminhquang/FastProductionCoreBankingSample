using CoreBanking.Domain.Core.Models;
using CoreBanking.Domain.Core.Services;

namespace CoreBanking.Infrastructure.Core
{
    public class FakeCurrencyConverter : ICurrencyConverter 
    {
        public Money Convert(Money amount, Currency currency)
        {
            return amount.Currency == currency ? amount : new Money(currency, amount.Value);
        }
    }
}