using System.ComponentModel.DataAnnotations;

namespace CoreBanking.Application.Core.DTOs
{
    public class CreateCustomerDto
    {
        [Required]
        public string FirstName { get; set; }

        [Required]
        public string LastName { get; set; }

        [Required]
        public string Email { get; set; }

        public string? DefaultCurrency { get; set; }
    }
}