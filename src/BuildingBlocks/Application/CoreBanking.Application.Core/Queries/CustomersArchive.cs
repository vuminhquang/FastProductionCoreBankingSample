using CoreBanking.Application.Core.Models;
using MediatR;

namespace CoreBanking.Application.Core.Queries
{
    public class CustomersArchive : IRequest<IEnumerable<CustomerArchiveItem>> { }
}