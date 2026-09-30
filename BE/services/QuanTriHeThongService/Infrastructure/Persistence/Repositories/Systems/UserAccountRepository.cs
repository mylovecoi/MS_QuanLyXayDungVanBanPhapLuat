using System.Security.Cryptography;
using QuanTriHeThongService.Domain.Entities.Systems;
using QuanTriHeThongService.Domain.Interfaces.Repositories;
using QuanTriHeThongService.Infrastructure.Persistence;
using QuanTriHeThongService.Infrastructure.Persistence.Entities;
using Microsoft.EntityFrameworkCore;

namespace QuanTriHeThongService.Infrastructure.Persistence.Repositories.Systems;

public class UserAccountRepository(QuanTriHeThongDbContext dbContext) : IUserAccountRepository
{
    private readonly QuanTriHeThongDbContext _dbContext = dbContext;

    public async Task<(IReadOnlyList<UserAccountEntity> Items, int TotalCount)> GetPagedAsync(string? search, int pageSize, int pageCurrent, string? level, CancellationToken cancellationToken = default)
    {
        var query = from user in _dbContext.Users.AsNoTracking()
                    join permissionGroup in _dbContext.GroupsPermision.AsNoTracking() on user.GroupPermissionId equals permissionGroup.Id into groupJoin
                    from permissionGroup in groupJoin.DefaultIfEmpty()
                    where !user.SSA
                    select new UserAccountEntity
                    {
                        Id = user.Id,
                        Username = user.Username,
                        Email = user.Email,
                        Name = user.Name,
                        Status = user.Status,
                        Content = user.Content,
                        GroupPermissionId = user.GroupPermissionId,
                        GroupPermissionName = permissionGroup != null ? permissionGroup.Name : null,
                        FirstLogin = user.FirstLogin,
                        Level = user.Level
                    };

        if (!string.IsNullOrWhiteSpace(search))
        {
            query = query.Where(x => x.Username.Contains(search) || x.Name.Contains(search) || x.Email.Contains(search));
        }

        if (!string.IsNullOrWhiteSpace(level))
        {
            query = query.Where(x => x.Level == level);
        }

        query = query.OrderBy(x => x.Username);
        var totalCount = await query.CountAsync(cancellationToken);
        var items = await query.Skip((pageCurrent - 1) * pageSize).Take(pageSize).ToListAsync(cancellationToken);
        return (items, totalCount);
    }

    public async Task<UserAccountEntity?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await (from user in _dbContext.Users.AsNoTracking()
                      join permissionGroup in _dbContext.GroupsPermision.AsNoTracking() on user.GroupPermissionId equals permissionGroup.Id into groupJoin
                      from permissionGroup in groupJoin.DefaultIfEmpty()
                      where user.Id == id && !user.SSA
                      select new UserAccountEntity
                      {
                          Id = user.Id,
                          Username = user.Username,
                          Email = user.Email,
                          Name = user.Name,
                          Status = user.Status,
                          Content = user.Content,
                          GroupPermissionId = user.GroupPermissionId,
                          GroupPermissionName = permissionGroup != null ? permissionGroup.Name : null,
                          FirstLogin = user.FirstLogin,
                          Level = user.Level
                      }).FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<OptionItemEntity>> GetGroupPermissionOptionsAsync(CancellationToken cancellationToken = default)
    {
        return await _dbContext.GroupsPermision.AsNoTracking()
            .Where(x => x.Status == "Kích hoạt")
            .OrderBy(x => x.Name)
            .Select(x => new OptionItemEntity
            {
                Value = x.Id.ToString(),
                DisplayName = x.Name
            })
            .ToListAsync(cancellationToken);
    }

    public Task<bool> ExistsByUsernameOrEmailAsync(string username, string email, Guid? excludeId = null, CancellationToken cancellationToken = default)
    {
        return _dbContext.Users.AsNoTracking()
            .Where(x => !x.SSA && (!excludeId.HasValue || x.Id != excludeId.Value))
            .AnyAsync(x => x.Username == username || x.Email == email, cancellationToken);
    }

    public Task<bool> GroupPermissionExistsAsync(Guid groupPermissionId, CancellationToken cancellationToken = default)
        => _dbContext.GroupsPermision.AsNoTracking().AnyAsync(x => x.Id == groupPermissionId, cancellationToken);

    public async Task UpdateAsync(UserAccountEntity entity, string? newPassword, CancellationToken cancellationToken = default)
    {
        var dbEntity = await _dbContext.Users.FirstAsync(x => x.Id == entity.Id && !x.SSA, cancellationToken);
        dbEntity.Name = entity.Name;
        dbEntity.Email = entity.Email;
        dbEntity.Status = entity.Status;
        dbEntity.Content = entity.Content;
        dbEntity.GroupPermissionId = entity.GroupPermissionId;
        if (!string.IsNullOrWhiteSpace(newPassword))
        {
            dbEntity.Password = BCrypt.Net.BCrypt.HashPassword(newPassword);
        }

        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task DuplicateAsync(Guid sourceId, string username, string name, string email, CancellationToken cancellationToken = default)
    {
        var source = await _dbContext.Users.AsNoTracking().FirstAsync(x => x.Id == sourceId && !x.SSA, cancellationToken);
        var entity = new User
        {
            Username = username,
            Email = email,
            Name = name,
            Password = BCrypt.Net.BCrypt.HashPassword("Life@2012!"),
            SSA = false,
            DanhMucDonViId = source.DanhMucDonViId,
            DoanhNghiepId = source.DoanhNghiepId,
            OTPSecretKey = GenerateSecretKey(),
            Status = source.Status,
            FirstLogin = true,
            LoginCount = 0,
            TenDonViBaoCao = source.TenDonViBaoCao,
            TenDonViChuQuanBaoCao = source.TenDonViChuQuanBaoCao,
            DiaDanh = source.DiaDanh,
            ChucDanhKy = source.ChucDanhKy,
            HoTenNguoiKy = source.HoTenNguoiKy,
            KyHieuDonVi = source.KyHieuDonVi,
            Content = source.Content,
            Menu = source.Menu,
            Theme = source.Theme,
            GroupPermissionId = source.GroupPermissionId,
            VNId = source.VNId,
            AgentId = source.AgentId,
            ScanDeviceId = source.ScanDeviceId,
            ScanDeviceName = source.ScanDeviceName,
            Level = source.Level
        };

        _dbContext.Users.Add(entity);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task ResetPasswordAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var dbEntity = await _dbContext.Users.FirstAsync(x => x.Id == id && !x.SSA, cancellationToken);
        dbEntity.Password = BCrypt.Net.BCrypt.HashPassword("Life@2012!");
        dbEntity.FirstLogin = true;
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task ChangeStatusAsync(Guid id, string status, CancellationToken cancellationToken = default)
    {
        var dbEntity = await _dbContext.Users.FirstAsync(x => x.Id == id && !x.SSA, cancellationToken);
        dbEntity.Status = status;
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        await _dbContext.Users.Where(x => x.Id == id && !x.SSA).ExecuteDeleteAsync(cancellationToken);
    }

    private static string GenerateSecretKey()
    {
        const string alphabet = "ABCDEFGHIJKLMNOPQRSTUVWXYZ234567";
        Span<byte> buffer = stackalloc byte[20];
        RandomNumberGenerator.Fill(buffer);

        var chars = new char[32];
        for (var i = 0; i < chars.Length; i++)
        {
            chars[i] = alphabet[buffer[i % buffer.Length] % alphabet.Length];
        }

        return new string(chars);
    }
}

