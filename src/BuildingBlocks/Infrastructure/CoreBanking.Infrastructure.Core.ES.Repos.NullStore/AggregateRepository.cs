using EventSourcing;
using OrderingServer.Domain.EventSourcing.Abstractions.Models;

namespace CoreBanking.Infrastructure.Core.ES.Repos.NullStore;

public class AggregateRepository<TA, TKey> : IAggregateRepository<TA, TKey>
    where TA : class, IAggregateRoot<TKey>
{
    public Task PersistAsync(TA aggregateRoot, CancellationToken cancellationToken = default)
    {
        // Do nothing
        return Task.CompletedTask;
    }

    public Task<TA> RehydrateAsync(TKey key, CancellationToken cancellationToken = default)
    {
        // Do nothing
        return null;
    }
}