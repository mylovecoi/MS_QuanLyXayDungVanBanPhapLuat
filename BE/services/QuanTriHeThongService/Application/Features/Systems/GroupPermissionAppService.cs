using QuanTriHeThongService.Application.Abstractions;
using QuanTriHeThongService.Application.Common.Models;
using QuanTriHeThongService.Application.DTOs.Systems;
using QuanTriHeThongService.Domain.Entities.Systems;
using QuanTriHeThongService.Domain.Interfaces.Repositories;

namespace QuanTriHeThongService.Application.Features.Systems;

public class GroupPermissionAppService(IGroupPermissionRepository repository) : IGroupPermissionAppService
{
    private readonly IGroupPermissionRepository _repository = repository;

    public async Task<PagedResult<GroupPermissionDto>> GetPagedAsync(string? search, int pageSize, int pageCurrent, CancellationToken cancellationToken = default)
    {
        var result = await _repository.GetPagedAsync(search, pageSize, pageCurrent, cancellationToken);
        return new PagedResult<GroupPermissionDto>
        {
            Items = result.Items.Select(x => x.ToDto()).ToArray(),
            TotalCount = result.TotalCount,
            PageSize = pageSize,
            PageCurrent = pageCurrent
        };
    }

    public async Task<GroupPermissionDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var entity = await _repository.GetByIdAsync(id, cancellationToken);
        return entity?.ToDto();
    }

    public async Task<IReadOnlyList<OptionItemDto>> GetTemplateGroupsAsync(CancellationToken cancellationToken = default)
    {
        var items = await _repository.GetTemplateGroupsAsync(cancellationToken);
        return items.Select(x => x.ToDto()).ToArray();
    }

    public async Task<(bool IsSuccess, string Message, GroupPermissionDto? Data)> CreateAsync(UpsertGroupPermissionRequest request, CancellationToken cancellationToken = default)
    {
        var normalized = Normalize(request);
        var validation = await ValidateAsync(normalized, null, true, cancellationToken);
        if (!validation.IsSuccess)
        {
            return (false, validation.Message, null);
        }

        var entity = new GroupPermissionEntity
        {
            Name = normalized.Name,
            Description = normalized.Description,
            Status = normalized.Status
        };

        await _repository.AddAsync(entity, cancellationToken);
        await _repository.InitializePermissionsAsync(entity.Id, normalized.TemplateGroup!, cancellationToken);
        var created = await _repository.GetByIdAsync(entity.Id, cancellationToken);
        return (true, "Tạo mới nhóm quyền thành công.", created?.ToDto());
    }

    public async Task<(bool IsSuccess, string Message, GroupPermissionDto? Data)> UpdateAsync(Guid id, UpsertGroupPermissionRequest request, CancellationToken cancellationToken = default)
    {
        var current = await _repository.GetByIdAsync(id, cancellationToken);
        if (current == null)
        {
            return (false, "Không tìm thấy nhóm quyền cần cập nhật.", null);
        }

        var normalized = Normalize(request);
        var validation = await ValidateAsync(normalized, id, false, cancellationToken);
        if (!validation.IsSuccess)
        {
            return (false, validation.Message, null);
        }

        current.Name = normalized.Name;
        current.Description = normalized.Description;
        current.Status = normalized.Status;
        await _repository.UpdateAsync(current, cancellationToken);

        var updated = await _repository.GetByIdAsync(id, cancellationToken);
        return (true, "Cập nhật nhóm quyền thành công.", updated?.ToDto());
    }

    public async Task<(bool IsSuccess, string Message)> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var current = await _repository.GetByIdAsync(id, cancellationToken);
        if (current == null)
        {
            return (false, "Không tìm thấy nhóm quyền cần xóa.");
        }

        await _repository.DeleteAsync(id, cancellationToken);
        return (true, "Xóa nhóm quyền thành công.");
    }

    public async Task<PagedResult<PermissionDto>> GetPermissionsAsync(Guid groupId, string? search, int pageSize, int pageCurrent, CancellationToken cancellationToken = default)
    {
        var result = await _repository.GetPermissionsAsync(groupId, search, pageSize, pageCurrent, cancellationToken);
        return new PagedResult<PermissionDto>
        {
            Items = result.Items.Select(x => x.ToDto()).ToArray(),
            TotalCount = result.TotalCount,
            PageSize = pageSize,
            PageCurrent = pageCurrent
        };
    }

    public async Task<PermissionDto?> GetPermissionByIdAsync(Guid groupId, Guid permissionId, CancellationToken cancellationToken = default)
    {
        var entity = await _repository.GetPermissionByIdAsync(permissionId, cancellationToken);
        if (entity == null || entity.GroupPermissionId != groupId)
        {
            return null;
        }

        return entity?.ToDto();
    }

    public async Task<(bool IsSuccess, string Message, PermissionDto? Data)> UpdatePermissionAsync(Guid groupId, Guid permissionId, UpdatePermissionRequest request, CancellationToken cancellationToken = default)
    {
        var current = await _repository.GetPermissionByIdAsync(permissionId, cancellationToken);
        if (current == null)
        {
            return (false, "Không tìm thấy quyền chi tiết cần cập nhật.", null);
        }

        if (current.GroupPermissionId != groupId)
        {
            return (false, "Quyền chi tiết không thuộc nhóm quyền đang cập nhật.", null);
        }

        current.Index = request.Index;
        current.Create = request.Index && request.Create && !IsGroup(current) && !IsLog(current);
        current.Edit = request.Index && request.Edit && !IsGroup(current) && !IsLog(current);
        current.Delete = request.Index && request.Delete && !IsGroup(current) && !IsLog(current);
        current.Approve = request.Index && request.Approve && CanUseApprove(current);
        current.Public = request.Index && request.Public && CanUseApprove(current);

        await _repository.UpdatePermissionAsync(current, cancellationToken);
        var updated = await _repository.GetPermissionByIdAsync(permissionId, cancellationToken);
        return (true, "Cập nhật quyền chi tiết thành công.", updated?.ToDto());
    }

    private async Task<(bool IsSuccess, string Message)> ValidateAsync(UpsertGroupPermissionRequest request, Guid? ignoreId, bool requireTemplateGroup, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.Name))
        {
            return (false, "Nhóm quyền là bắt buộc.");
        }

        if (string.IsNullOrWhiteSpace(request.Status))
        {
            return (false, "Trạng thái là bắt buộc.");
        }

        if (requireTemplateGroup && string.IsNullOrWhiteSpace(request.TemplateGroup))
        {
            return (false, "Nhóm quyền mẫu là bắt buộc khi tạo mới.");
        }

        if (await _repository.ExistsByNameAsync(request.Name, ignoreId, cancellationToken))
        {
            return (false, "Tên nhóm quyền đã tồn tại.");
        }

        return (true, "Dữ liệu hợp lệ.");
    }

    private static UpsertGroupPermissionRequest Normalize(UpsertGroupPermissionRequest request)
    {
        request.Name = request.Name.Trim();
        request.Description = string.IsNullOrWhiteSpace(request.Description) ? null : request.Description.Trim();
        request.Status = request.Status.Trim();
        request.TemplateGroup = string.IsNullOrWhiteSpace(request.TemplateGroup) ? null : request.TemplateGroup.Trim();
        return request;
    }

    private static bool IsGroup(PermissionEntity entity)
        => string.Equals(entity.PhanLoai, "Group", StringComparison.OrdinalIgnoreCase);

    private static bool IsLog(PermissionEntity entity)
        => entity.Role?.Contains("Log", StringComparison.OrdinalIgnoreCase) == true;

    private static bool CanUseApprove(PermissionEntity entity)
    {
        return !IsGroup(entity)
               && !IsLog(entity)
               && entity.Role?.Contains("Systems", StringComparison.OrdinalIgnoreCase) != true
               && entity.Role?.Contains("Settings", StringComparison.OrdinalIgnoreCase) != true;
    }
}

