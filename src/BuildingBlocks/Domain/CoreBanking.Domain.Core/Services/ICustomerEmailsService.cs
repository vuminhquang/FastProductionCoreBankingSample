namespace CoreBanking.Domain.Core.Services
{
    public interface ICustomerEmailsService
    {
        Task<bool> ExistsAsync(string email);
        Task CreateAsync(string email, Guid customerId);
    }
}