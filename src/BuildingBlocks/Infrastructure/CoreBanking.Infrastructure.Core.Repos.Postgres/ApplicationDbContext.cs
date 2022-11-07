using CoreBanking.Infrastructure.Core.Repos.Postgres.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace CoreBanking.Infrastructure.Core.Repos.Postgres;

public class ApplicationDbContext : DbContext
{
    public DbSet<CustomerEntity> Customers => Set<CustomerEntity>();
    
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }

    #region Audit, not in this POC version
    private readonly IHttpContextAccessor _httpContextAccessor;
    
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options,
            IHttpContextAccessor httpContextAccessor) : base(options)
        {
            _httpContextAccessor = httpContextAccessor;
        }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
    }

    /// <summary>
    ///     SaveChangesAsync with entities audit
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        var entries = ChangeTracker
            .Entries()
            .Where(e => e.Entity is IAuditedEntityBase && (
                e.State == EntityState.Added
                || e.State == EntityState.Modified));

        var modifiedOrCreatedBy = _httpContextAccessor?.HttpContext?.User?.Identity?.Name ?? "System";

        foreach (var entityEntry in entries)
        {
            if (entityEntry.State == EntityState.Added)
            {
                ((IAuditedEntityBase) entityEntry.Entity).CreatedDate = DateTime.UtcNow;
                ((IAuditedEntityBase) entityEntry.Entity).CreatedBy = modifiedOrCreatedBy;
            }
            else
            {
                Entry((IAuditedEntityBase) entityEntry.Entity).Property(p => p.CreatedDate).IsModified = false;
                Entry((IAuditedEntityBase) entityEntry.Entity).Property(p => p.CreatedBy).IsModified = false;
            }

            ((IAuditedEntityBase) entityEntry.Entity).LastModifiedDate = DateTime.UtcNow;
            ((IAuditedEntityBase) entityEntry.Entity).LastModifiedBy = modifiedOrCreatedBy;
        }

        return await base.SaveChangesAsync(cancellationToken);
    }

    #endregion
}