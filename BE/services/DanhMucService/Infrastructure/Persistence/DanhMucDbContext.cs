using BuildingBlocks.Abstractions;
using DanhMucService.Infrastructure.Persistence.Entities;
using Microsoft.EntityFrameworkCore;

namespace DanhMucService.Infrastructure.Persistence;

public class DanhMucDbContext : DbContext
{
    private readonly ICurrentUserContext? _currentUserContext;

    public DanhMucDbContext(DbContextOptions<DanhMucDbContext> options, ICurrentUserContext? currentUserContext = null)
        : base(options)
    {
        _currentUserContext = currentUserContext;
    }

    public DbSet<DanhMucDonVi> DanhMucDonVis { get; set; }
    public DbSet<DanhMucDiaDanh> DanhMucDiaDanhs { get; set; }
    public DbSet<DanhMucQuyTrinhSoanThao> DanhMucQuyTrinhSoanThaos { get; set; }
    public DbSet<DanhMucBuocQuyTrinh> DanhMucBuocQuyTrinhs { get; set; }
    public DbSet<DanhMucChuyenBuocQuyTrinh> DanhMucChuyenBuocQuyTrinhs { get; set; }
    public DbSet<DanhMucTrangThai> DanhMucTrangThais { get; set; }
    public DbSet<DanhMucTieuChiDiem> DanhMucTieuChiDiems { get; set; }
    public DbSet<DanhMucTieuChiDiemMuc> DanhMucTieuChiDiemMucs { get; set; }
    public DbSet<DanhMucVanBan> DanhMucVanBans { get; set; }
    public DbSet<DanhMucLinhVuc> DanhMucLinhVucs { get; set; }
    public DbSet<DanhMucPhongBan> DanhMucPhongBans { get; set; }
    public DbSet<DanhMucCanBo> DanhMucCanBos { get; set; }
    public DbSet<HoSoVanBanUsage> HoSoVanBanUsages { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.HasDefaultSchema("dm");

        modelBuilder.Entity<DanhMucBuocQuyTrinh>()
            .HasOne<DanhMucQuyTrinhSoanThao>()
            .WithMany()
            .HasForeignKey(x => x.QuyTrinhSoanThaoId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<DanhMucBuocQuyTrinh>()
            .HasOne<DanhMucDonVi>()
            .WithMany()
            .HasForeignKey(x => x.DonViTiepNhanMacDinhId)
            .OnDelete(DeleteBehavior.NoAction);

        modelBuilder.Entity<DanhMucChuyenBuocQuyTrinh>()
            .HasOne<DanhMucQuyTrinhSoanThao>()
            .WithMany()
            .HasForeignKey(x => x.QuyTrinhSoanThaoId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<DanhMucChuyenBuocQuyTrinh>()
            .HasOne<DanhMucBuocQuyTrinh>()
            .WithMany()
            .HasForeignKey(x => x.TuBuocId)
            .OnDelete(DeleteBehavior.NoAction);

        modelBuilder.Entity<DanhMucChuyenBuocQuyTrinh>()
            .HasOne<DanhMucBuocQuyTrinh>()
            .WithMany()
            .HasForeignKey(x => x.DenBuocId)
            .OnDelete(DeleteBehavior.NoAction);

        modelBuilder.Entity<DanhMucQuyTrinhSoanThao>()
            .HasOne<DanhMucVanBan>()
            .WithMany()
            .HasForeignKey(x => x.DanhMucVanBanId)
            .OnDelete(DeleteBehavior.NoAction);

        // Transitional read guard: this should move behind a VanBan service API/read model
        // when that bounded context exposes an API or read model owned by the VanBan service.
        modelBuilder.Entity<HoSoVanBanUsage>().ToTable("HoSoVanBans", "dbo", tableBuilder => tableBuilder.ExcludeFromMigrations());
        modelBuilder.Entity<DanhMucTieuChiDiem>().Property(x => x.DiemToiDa).HasPrecision(18, 2);
        modelBuilder.Entity<DanhMucTieuChiDiemMuc>().Property(x => x.TuGiaTri).HasPrecision(18, 2);
        modelBuilder.Entity<DanhMucTieuChiDiemMuc>().Property(x => x.DenGiaTri).HasPrecision(18, 2);
        modelBuilder.Entity<DanhMucTieuChiDiemMuc>().Property(x => x.Diem).HasPrecision(18, 2);
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
