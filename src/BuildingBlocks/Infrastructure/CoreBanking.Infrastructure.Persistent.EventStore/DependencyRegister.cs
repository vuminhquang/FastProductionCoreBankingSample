using CoreBanking.Domain.Core.Models;
using EngineFramework;
using EventSourcing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using OrderingServer.Domain.EventSourcing.Abstractions;
using OrderingServer.Domain.EventSourcing.Abstractions.Models;

namespace CoreBanking.Infrastructure.Persistent.EventStore;

public class DependencyRegister : IDependencyRegister
{
    public void ServicesRegister(IServiceCollection services)
    {
        var connectionString = IDependencyRegister.Configuration.GetConnectionString("eventstore");
        services.AddSingleton<IEventStoreConnectionWrapper>(ctx =>
            {
                var logger = ctx.GetRequiredService<ILogger<EventStoreConnectionWrapper>>();
                return new EventStoreConnectionWrapper(new Uri(connectionString), logger);
            }).AddEventsRepository<Customer, Guid>()
            .AddEventsRepository<Account, Guid>();
    }
}

public static class Extensions
{
    public static IServiceCollection AddEventsRepository<TA, TK>(this IServiceCollection services)
        where TA : class, IAggregateRoot<TK>
    {
        return services.AddSingleton<IAggregateRepository<TA, TK>>(ctx =>
        {
            var connectionWrapper = ctx.GetRequiredService<IEventStoreConnectionWrapper>();
            var eventDeserializer = ctx.GetRequiredService<IEventSerializer>();
            return new AggregateRepository<TA, TK>(connectionWrapper, eventDeserializer);
        });
    }
}