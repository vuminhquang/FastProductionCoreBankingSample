using System.Diagnostics;
using CoreBanking.Domain.Core.Services;
using CoreBanking.Infrastructure.Core.Repos.Postgres.Repositories;
using CoreBanking.Infrastructure.Core.Repos.Postgres.Services;
using EngineFramework;
using FreeBot.Domain.Repositories.Interfaces;
using Lamar;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using RepositoryHelper;

namespace CoreBanking.Infrastructure.Core.Repos.Postgres;

public class DependencyRegister : IDependencyRegister, IAppConfigure
{
    public void ServicesRegister(IServiceCollection services)
    {
        // Needed for configure method to be called at startup
        services.AddTransient<IAppConfigure, DependencyRegister>();
        
        #region DbContext

        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddDbContext<ApplicationDbContext>(opts =>
        {
            // Set the active provider via configuration
            Debug.Assert(IDependencyRegister.Configuration != null, "IDependencyRegister.Configuration != null");
            var configuration = IDependencyRegister.Configuration;
            var provider = configuration.GetValue("Provider", "Postgres");
            
            _ = provider switch
            {
                // "Sqlite" => opts.UseSqlite(
                //     configuration.GetConnectionString("SqliteConnection"),
                //     x => x.MigrationsAssembly("SqliteMigrations")),
                
                "InMemoryDb" => opts.UseInMemoryDatabase(
                    configuration.GetConnectionString("SqliteConnection")),
                
                "Postgres" => opts.UseNpgsql(configuration.GetConnectionString("Default"),
                    optionsBuilder =>
                    {
                        optionsBuilder.MigrationsAssembly("CoreBanking.Infrastructure.Core.Repos.Postgres");
                    }).EnableSensitiveDataLogging(true),

                _ => throw new Exception($"Unsupported provider: {provider}")
            };
        });
        services.AddScoped<DbContext>(provider => provider.GetRequiredService<ApplicationDbContext>());

        #endregion
        
        // Other services register
        services.AddScoped<ICustomerEmailsService, CustomerEmailsService>();
        services.AddScoped<ReadOnlyCustomerRepository>();
        services.AddScoped<CustomerRepository>();

        // Add the event handlers
        var svcs = (ServiceRegistry)services;
        svcs.Scan(scan =>
        {
            scan.Assembly(typeof(DependencyRegister).Assembly);
            scan.AddAllTypesOf(typeof(INotificationHandler<>));
            
            // Not to add these request handler as this is not design to handle here
            // scan.AddAllTypesOf(typeof(IRequestHandler<>));
            // scan.AddAllTypesOf(typeof(IRequestHandler<,>));
        }); 
    }

    public void Configure(IHost app)
    {
        // Auto migration
        using var scope = app.Services.CreateScope();
        var dataContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        dataContext.Database.Migrate();
    }
}