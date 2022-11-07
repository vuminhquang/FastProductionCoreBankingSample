using EventStore.ClientAPI;

namespace CoreBanking.Infrastructure.Core.ES.Repos.EventStore
{
    public interface IEventStoreConnectionWrapper
    {
        Task<IEventStoreConnection> GetConnectionAsync();
    }
}