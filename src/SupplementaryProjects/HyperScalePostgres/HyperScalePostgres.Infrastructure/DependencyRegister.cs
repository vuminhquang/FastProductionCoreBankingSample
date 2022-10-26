using EngineFramework;
using Microsoft.Extensions.DependencyInjection;

namespace HyperScalePostgres.Infrastructure;

public class DependencyRegister : IDependencyRegister
{
    public void ServicesRegister(IServiceCollection services)
    {
        services.AddHostedService<PostgresBackgroundService>();
    }
}