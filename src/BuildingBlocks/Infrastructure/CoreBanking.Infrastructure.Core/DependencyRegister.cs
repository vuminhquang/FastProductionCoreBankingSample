using CoreBanking.Domain.Core.Services;
using EngineFramework;
using Microsoft.Extensions.DependencyInjection;

namespace CoreBanking.Infrastructure.Core;

public class DependencyRegister : IDependencyRegister
{
    public void ServicesRegister(IServiceCollection services)
    {
        services.AddTransient<ICurrencyConverter, FakeCurrencyConverter>();
    }
}