using QuanTriHeThongService.Application.Abstractions;
using QuanTriHeThongService.Application.Common.Models;
using QuanTriHeThongService.Application.DTOs.Systems;
using QuanTriHeThongService.Domain.Entities.Systems;
using QuanTriHeThongService.Domain.Interfaces.Repositories;

namespace QuanTriHeThongService.Application.Features.Systems;

public class RoleActionAppService(IRoleActionRepository repository) : IRoleActionAppService
{
    private readonly IRoleActionRepository _repository = repository;

    public async Task<PagedResult<RoleActionDto>> GetPagedAsync(string? search, int pageSize, int pageCurrent, CancellationToken cancellationToken = default)
    {
        var result = await _repository.GetPagedAsync(search, pageSize, pageCurrent, cancellationToken);
        return new PagedResult<RoleActionDto>
        {
            Items = result.Items.Select(x => x.ToDto()).ToArray(),
            TotalCount = result.TotalCount,
            PageSize = pageSize,
            PageCurrent = pageCurrent
        };
    }

    public async Task<IReadOnlyList<RoleActionDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var items = await _repository.GetAllAsync(cancellationToken);
        return items.Select(x => x.ToDto()).ToArray();
    }

    public async Task<IReadOnlyList<RoleActionDto>> GetGroupOptionsAsync(CancellationToken cancellationToken = default)
    {
        var items = await _repository.GetGroupOptionsAsync(cancellationToken);
        return items.Select(x => x.ToDto()).ToArray();
    }

    public async Task<RoleActionDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var item = await _repository.GetByIdAsync(id, cancellationToken);
        return item?.ToDto();
    }

    public Task<int> GetNextSortOrderAsync(Guid? parentId, CancellationToken cancellationToken = default)
        => _repository.GetNextSortOrderAsync(parentId, cancellationToken);

    public async Task<(bool IsSuccess, string Message, RoleActionDto? Data)> CreateAsync(UpsertRoleActionRequest request, CancellationToken cancellationToken = default)
    {
        var normalized = NormalizeRequest(request);
        var validation = await ValidateAsync(normalized, null, cancellationToken);
        if (!validation.IsSuccess)
        {
            return (false, validation.Message, null);
        }

        var entity = normalized.ToEntity();
        await ApplyHierarchyAsync(entity, normalized.ParentId, cancellationToken);
        await _repository.AddAsync(entity, cancellationToken);

        var created = await _repository.GetByIdAsync(entity.Id, cancellationToken);
        return (true, "Tạo mới chức năng thành công.", created?.ToDto());
    }

    public async Task<(bool IsSuccess, string Message, RoleActionDto? Data)> UpdateAsync(Guid id, UpsertRoleActionRequest request, CancellationToken cancellationToken = default)
    {
        var current = await _repository.GetByIdAsync(id, cancellationToken);
        if (current == null)
        {
            return (false, "Không tìm thấy chức năng cần cập nhật.", null);
        }

        var normalized = NormalizeRequest(request);
        normalized.Id = id;
        var validation = await ValidateAsync(normalized, id, cancellationToken);
        if (!validation.IsSuccess)
        {
            return (false, validation.Message, null);
        }

        current.STTSapXep = normalized.STTSapXep;
        current.PhanLoai = normalized.PhanLoai;
        current.Role = normalized.Role;
        current.Title = normalized.Title;
        current.Controller = normalized.PhanLoai == "Detail" ? normalized.Controller : null;
        current.Action = normalized.PhanLoai == "Detail" ? normalized.Action : null;
        current.Parameter = normalized.Parameter;
        current.Table = normalized.PhanLoai == "Detail" ? normalized.Table : null;
        current.Status = normalized.Status;
        current.UseGroup = normalized.UseGroup;
        current.FrontendPath = normalized.FrontendPath;
        current.IsVisibleInMenu = normalized.IsVisibleInMenu;
        current.ClientApp = normalized.ClientApp;
        current.MenuTitle = normalized.MenuTitle;
        current.MenuIcon = normalized.MenuIcon;
        current.Icon = normalized.Icon;

        await ApplyHierarchyAsync(current, normalized.ParentId, cancellationToken);
        await _repository.UpdateAsync(current, cancellationToken);

        var updated = await _repository.GetByIdAsync(id, cancellationToken);
        return (true, "Cập nhật chức năng thành công.", updated?.ToDto());
    }

    public async Task<(bool IsSuccess, string Message)> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var current = await _repository.GetByIdAsync(id, cancellationToken);
        if (current == null)
        {
            return (false, "Không tìm thấy chức năng cần xóa.");
        }

        if (await _repository.HasChildrenAsync(id, cancellationToken))
        {
            return (false, "Chức năng đang có chức năng con, không thể xóa.");
        }

        await _repository.DeleteAsync(id, cancellationToken);
        return (true, "Xóa chức năng thành công.");
    }

    private async Task<(bool IsSuccess, string Message)> ValidateAsync(UpsertRoleActionRequest request, Guid? ignoreId, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.Role))
        {
            return (false, "Quyền là bắt buộc.");
        }

        if (string.IsNullOrWhiteSpace(request.Title))
        {
            return (false, "Mô tả chức năng là bắt buộc.");
        }

        if (string.IsNullOrWhiteSpace(request.PhanLoai) || (request.PhanLoai != "Group" && request.PhanLoai != "Detail"))
        {
            return (false, "Phân loại phải là Group hoặc Detail.");
        }

        if (string.IsNullOrWhiteSpace(request.Status))
        {
            return (false, "Trạng thái là bắt buộc.");
        }

        if (request.STTSapXep <= 0)
        {
            return (false, "Sắp xếp phải lớn hơn 0.");
        }

        if (!IsValidClientApp(request.ClientApp))
        {
            return (false, "Ứng dụng hiển thị chỉ chấp nhận frontend, legacy hoặc both.");
        }

        if (await _repository.ExistsByRoleAsync(request.Role, ignoreId, cancellationToken))
        {
            return (false, "Quyền đã tồn tại.");
        }

        if (request.ParentId.HasValue)
        {
            if (ignoreId.HasValue && request.ParentId.Value == ignoreId.Value)
            {
                return (false, "Không thể chọn chính chức năng hiện tại làm cha.");
            }

            var parent = await _repository.GetByIdAsync(request.ParentId.Value, cancellationToken);
            if (parent == null)
            {
                return (false, "Không tìm thấy chức năng cha.");
            }

            if (!string.Equals(parent.PhanLoai, "Group", StringComparison.OrdinalIgnoreCase))
            {
                return (false, "Chỉ có thể gán vào chức năng cha loại Group.");
            }

            if (ignoreId.HasValue && await _repository.IsDescendantAsync(ignoreId.Value, request.ParentId.Value, cancellationToken))
            {
                return (false, "Không thể gán chức năng cha là nhánh con của chính nó.");
            }
        }

        return (true, "Dữ liệu hợp lệ.");
    }

    private async Task ApplyHierarchyAsync(RoleActionEntity entity, Guid? parentId, CancellationToken cancellationToken)
    {
        if (!parentId.HasValue)
        {
            entity.ParentId = null;
            entity.ParentTitle = null;
            entity.Level = 0;
            return;
        }

        var parent = await _repository.GetByIdAsync(parentId.Value, cancellationToken)
                     ?? throw new InvalidOperationException("Parent role action not found.");

        entity.ParentId = parent.Id;
        entity.ParentTitle = parent.Title;
        entity.Level = parent.Level + 1;
    }

    private static UpsertRoleActionRequest NormalizeRequest(UpsertRoleActionRequest request)
    {
        request.Role = request.Role.Trim();
        request.Title = request.Title?.Trim();
        request.PhanLoai = string.IsNullOrWhiteSpace(request.PhanLoai) ? "Group" : request.PhanLoai.Trim();
        request.Controller = string.IsNullOrWhiteSpace(request.Controller) ? null : request.Controller.Trim();
        request.Action = string.IsNullOrWhiteSpace(request.Action) ? null : request.Action.Trim();
        request.Parameter = string.IsNullOrWhiteSpace(request.Parameter) ? null : request.Parameter.Trim();
        request.Table = string.IsNullOrWhiteSpace(request.Table) ? null : request.Table.Trim();
        request.Status = request.Status.Trim();
        request.UseGroup = string.IsNullOrWhiteSpace(request.UseGroup) ? null : request.UseGroup.Trim();
        request.FrontendPath = string.IsNullOrWhiteSpace(request.FrontendPath) ? null : NormalizePath(request.FrontendPath);
        request.ClientApp = NormalizeClientApp(request.ClientApp);
        request.MenuTitle = string.IsNullOrWhiteSpace(request.MenuTitle) ? null : request.MenuTitle.Trim();
        request.MenuIcon = string.IsNullOrWhiteSpace(request.MenuIcon) ? null : request.MenuIcon.Trim();
        request.Icon = string.IsNullOrWhiteSpace(request.Icon) ? null : request.Icon.Trim();

        if (!request.ParentId.HasValue || request.ParentId == Guid.Empty)
        {
            request.ParentId = null;
        }

        return request;
    }

    private static string NormalizePath(string path)
    {
        var trimmed = path.Trim();
        return trimmed.StartsWith("/", StringComparison.Ordinal) ? trimmed : "/" + trimmed;
    }

    private static string? NormalizeClientApp(string? clientApp)
    {
        if (string.IsNullOrWhiteSpace(clientApp))
        {
            return null;
        }

        var normalized = clientApp.Trim().ToLowerInvariant();
        return normalized switch
        {
            "frontend" => "frontend",
            "frontend.web" => "frontend",
            "frontendweb" => "frontend",
            "legacy" => "legacy",
            "ui cu" => "legacy",
            "uicu" => "legacy",
            "both" => "both",
            "ca hai" => "both",
            "cahai" => "both",
            _ => normalized
        };
    }

    private static bool IsValidClientApp(string? clientApp)
    {
        return clientApp is null
               || string.Equals(clientApp, "frontend", StringComparison.OrdinalIgnoreCase)
               || string.Equals(clientApp, "legacy", StringComparison.OrdinalIgnoreCase)
               || string.Equals(clientApp, "both", StringComparison.OrdinalIgnoreCase);
    }
}

