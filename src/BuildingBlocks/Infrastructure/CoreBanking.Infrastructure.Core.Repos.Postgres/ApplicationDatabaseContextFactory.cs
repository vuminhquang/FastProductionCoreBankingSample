using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace CoreBanking.Infrastructure.Core.Repos.Postgres;

//To create migration only
public class ApplicationDatabaseContextFactory : IDesignTimeDbContextFactory<ApplicationDbContext>
{
    public ApplicationDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
        var connectionString = args.Any() 
            ? args[0] 
            : "Server=192.168.10.50:5432;User ID=postgres;Password=Pass@word1;Database=CoreBanking;Pooling=true;";
        optionsBuilder.UseNpgsql(connectionString);

        return new ApplicationDbContext(optionsBuilder.Options);
    }
}