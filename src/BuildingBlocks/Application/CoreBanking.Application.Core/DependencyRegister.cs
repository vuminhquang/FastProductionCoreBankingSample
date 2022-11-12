using CoreBanking.Application.Core.Services;
using EngineFramework;
using Microsoft.Extensions.DependencyInjection;

namespace CoreBanking.Application.Core;

public class DependencyRegister : IDependencyRegister
{
    public void ServicesRegister(IServiceCollection services)
    {
        services.AddTransient<CustomersService>();
    }
}