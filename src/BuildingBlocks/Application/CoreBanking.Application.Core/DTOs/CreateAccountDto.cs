using System.ComponentModel.DataAnnotations;

namespace CoreBanking.Application.Core.DTOs
{
    public class CreateAccountDto
    {
        [Required]
        public string CurrencyCode { get; set; }

        [Required]
        public Guid CustomerId { get; set; } 
    }
}