using CoreBanking.Domain.Core.Models;
using EngineFramework;
using EventSourcing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using OrderingServer.Domain.EventSourcing.Abstractions;
using OrderingServer.Domain.EventSourcing.Abstractions.Models;

namespace CoreBanking.Infrastructure.Core.ES.Repos.NullStore;

public class DependencyRegister : IDependencyRegister
{
    public void ServicesRegister(IServiceCollection services)
    {
        services.AddEventsRepository<Customer, Guid>();
    }
}

public static class Extensions
{
    public static IServiceCollection AddEventsRepository<TA, TK>(this IServiceCollection services)
        where TA : class, IAggregateRoot<TK>
    {
        return services.AddSingleton<IAggregateRepository<TA, TK>>(ctx =>
        {
            var logger = ctx.GetRequiredService<ILogger<AggregateRepository<TA, TK>>>();
            return new AggregateRepository<TA, TK>(logger);
        });
    }
}