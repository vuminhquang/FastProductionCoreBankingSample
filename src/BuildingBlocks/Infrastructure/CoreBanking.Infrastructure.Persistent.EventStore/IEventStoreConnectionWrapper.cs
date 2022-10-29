using EventStore.ClientAPI;

namespace CoreBanking.Infrastructure.Persistent.EventStore
{
    public interface IEventStoreConnectionWrapper
    {
        Task<IEventStoreConnection> GetConnectionAsync();
    }
}