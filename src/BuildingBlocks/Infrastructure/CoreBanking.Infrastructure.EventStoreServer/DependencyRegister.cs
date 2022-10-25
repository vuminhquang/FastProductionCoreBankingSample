using EngineFramework;
using Microsoft.Extensions.DependencyInjection;

namespace CoreBanking.Infrastructure.EventStoreServer;

public class DependencyRegister : IDependencyRegister
{
    public void ServicesRegister(IServiceCollection services)
    {
        services.AddHostedService<EventStoreBackgroundService>();
    }
}