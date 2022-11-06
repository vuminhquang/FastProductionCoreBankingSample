using EngineFramework;
using FreeBot.Domain.Repositories.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using RepositoryHelper;

namespace CoreBanking.Infrastructure.Core.Repos.Postgres;

public class DependencyRegister : IDependencyRegister
{
    public void ServicesRegister(IServiceCollection services)
    {
        services.AddScoped<IUnitOfWork, UnitOfWork>();
    }
}