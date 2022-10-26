using System.ComponentModel.DataAnnotations;

namespace CoreBanking.Application.Core.DTOs
{
    public class WithdrawDto
    {
        [Required]
        public string CurrencyCode { get; set; }

        [Required, Range(0, double.MaxValue)]
        public decimal Amount { get; set; }
    }
}