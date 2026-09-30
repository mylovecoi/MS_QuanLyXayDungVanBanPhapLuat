using BuildingBlocks.Abstractions;
using QuanTriHeThongService.Infrastructure.Persistence.Entities;
using Microsoft.EntityFrameworkCore;

namespace QuanTriHeThongService.Infrastructure.Persistence;

public class QuanTriHeThongDbContext : DbContext
{
    private readonly ICurrentUserContext? _currentUserContext;

    public QuanTriHeThongDbContext(DbContextOptions<QuanTriHeThongDbContext> options, ICurrentUserContext? currentUserContext = null)
        : base(options)
    {
        _currentUserContext = currentUserContext;
    }

    public DbSet<Log> Logs { get; set; }
    public DbSet<SystemInfo> SystemInfo { get; set; }
    public DbSet<User> Users { get; set; }
    public DbSet<GroupPermision> GroupsPermision { get; set; }
    public DbSet<Permission> Permission { get; set; }
    public DbSet<RoleAction> RoleActions { get; set; }
    public DbSet<QuestionAnswer> QuestionAnswers { get; set; }
    public DbSet<OptionData> OptionDatas { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.HasDefaultSchema("qtht");
    }

    public override int SaveChanges()
    {
        UpdateAuditFields();
        return base.SaveChanges();
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        UpdateAuditFields();
        return await base.SaveChangesAsync(cancellationToken);
    }

    private void UpdateAuditFields()
    {
        var userId = _currentUserContext?.UserId;
        if (userId == null || userId == Guid.Empty)
        {
            return;
        }

        var entries = ChangeTracker.Entries()
            .Where(e => e.Entity is BaseEntity && (e.State == EntityState.Added || e.State == EntityState.Modified))
            .ToList();

        foreach (var entry in entries)
        {
            var entity = (BaseEntity)entry.Entity;
            entity.UpdatedDate = DateTime.Now;
            entity.UpdatedBy = userId.Value;

            if (entry.State == EntityState.Added)
            {
                entity.CreatedDate = DateTime.Now;
                entity.CreatedBy = userId.Value;
                continue;
            }

            entry.Property(nameof(BaseEntity.CreatedDate)).IsModified = false;
            entry.Property(nameof(BaseEntity.CreatedBy)).IsModified = false;
        }
    }
}
