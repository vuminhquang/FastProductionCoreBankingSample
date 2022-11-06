using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace CoreBanking.Infrastructure.Core.Repos.Postgres;

//To create migration only
public class ApplicationDatabaseContextFactory : IDesignTimeDbContextFactory<ApplicationDatabaseContext>
{
    public ApplicationDatabaseContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<ApplicationDatabaseContext>();
        var connectionString = args.Any() 
            ? args[0] 
            : "Server=192.68.10.50:5432;User ID=postgres;Password=Pass@word1;Database=CoreBanking;Pooling=true;";
        optionsBuilder.UseNpgsql(connectionString);

        return new ApplicationDatabaseContext(optionsBuilder.Options);
    }
}