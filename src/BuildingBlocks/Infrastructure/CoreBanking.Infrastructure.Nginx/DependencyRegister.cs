using EngineFramework;
using Microsoft.Extensions.DependencyInjection;

namespace CryptoPaymentGateway.Infrastructure.TorNgrok;

public class DependencyRegister : IDependencyRegister
{
    public void ServicesRegister(IServiceCollection services)
    {
        services.AddHostedService<NginxBackgroundService>();
    }
}