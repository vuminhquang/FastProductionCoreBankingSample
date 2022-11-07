using EngineFramework;
using EventSourcing.EventBus;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace CoreBanking.Infrastructure.Core.ES.EvProducer.NullProducer;

public class DependencyRegister : IDependencyRegister
{
    public void ServicesRegister(IServiceCollection services)
    {
        services.AddSingleton<IEventProducer>(ctx =>
        {
            var logger = ctx.GetRequiredService<ILogger<EventProducer>>();
            return new EventProducer(logger);
        });
    }
}