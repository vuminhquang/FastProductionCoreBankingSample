using CoreBanking.Domain.Core.Models;

namespace CoreBanking.Domain.Core.Services
{
    public interface ICurrencyConverter
    {
        Money Convert(Money amount, Currency currency);
    }
}