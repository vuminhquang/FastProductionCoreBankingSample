using EventSourcing;
using Microsoft.Extensions.Logging;
using OrderingServer.Domain.EventSourcing.Abstractions.Models;

namespace CoreBanking.Infrastructure.Core.ES.Repos.NullStore;

public class AggregateRepository<TA, TKey> : IAggregateRepository<TA, TKey>
    where TA : class, IAggregateRoot<TKey>
{
    private readonly ILogger<AggregateRepository<TA, TKey>> _logger;

    public AggregateRepository(ILogger<AggregateRepository<TA, TKey>> logger)
    {
        _logger = logger;
    }

    public Task PersistAsync(TA aggregateRoot, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("AggregateRepository PersistAsync {AggregateRoot}", aggregateRoot);
        // Do nothing
        return Task.CompletedTask;
    }

    public Task<TA> RehydrateAsync(TKey key, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("AggregateRepository RehydrateAsync {Key}", key);
        // Do nothing
        return null;
    }
}