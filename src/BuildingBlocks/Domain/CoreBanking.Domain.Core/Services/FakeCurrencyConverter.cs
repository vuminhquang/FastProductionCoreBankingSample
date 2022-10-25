using CoreBanking.Domain.Core.Models;

namespace CoreBanking.Domain.Core.Services
{
    public class FakeCurrencyConverter : ICurrencyConverter 
    {
        public Money Convert(Money amount, Currency currency)
        {
            return amount.Currency == currency ? amount : new Money(currency, amount.Value);
        }
    }
}