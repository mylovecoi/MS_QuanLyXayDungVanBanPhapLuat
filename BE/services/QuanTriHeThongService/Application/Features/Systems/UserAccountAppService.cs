using QuanTriHeThongService.Application.Abstractions;
using QuanTriHeThongService.Application.Common.Models;
using QuanTriHeThongService.Application.DTOs.Systems;
using QuanTriHeThongService.Domain.Interfaces.Repositories;

namespace QuanTriHeThongService.Application.Features.Systems;

public class UserAccountAppService(IUserAccountRepository repository) : IUserAccountAppService
{
    private readonly IUserAccountRepository _repository = repository;

    public async Task<PagedResult<UserAccountDto>> GetPagedAsync(string? search, int pageSize, int pageCurrent, string? level, CancellationToken cancellationToken = default)
    {
        pageCurrent = pageCurrent < 1 ? 1 : pageCurrent;
        pageSize = pageSize < 5 ? 5 : pageSize > 100 ? 100 : pageSize;

        var result = await _repository.GetPagedAsync(search?.Trim(), pageSize, pageCurrent, level?.Trim(), cancellationToken);
        return new PagedResult<UserAccountDto>
        {
            Items = result.Items.Select(x => x.ToDto()).ToList(),
            TotalCount = result.TotalCount,
            PageSize = pageSize,
            PageCurrent = pageCurrent
        };
    }

    public async Task<UserAccountDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        => (await _repository.GetByIdAsync(id, cancellationToken))?.ToDto();

    public async Task<IReadOnlyList<OptionItemDto>> GetGroupPermissionOptionsAsync(CancellationToken cancellationToken = default)
    {
        var items = await _repository.GetGroupPermissionOptionsAsync(cancellationToken);
        return items.Select(x => new OptionItemDto
        {
            Value = x.Value,
            DisplayName = x.DisplayName
        }).ToList();
    }

    public async Task<(bool IsSuccess, string Message, UserAccountDto? Data)> UpdateAsync(Guid id, UpdateUserAccountRequest request, CancellationToken cancellationToken = default)
    {
        var existing = await _repository.GetByIdAsync(id, cancellationToken);
        if (existing == null)
        {
            return (false, "Không tìm thấy tài khoản truy cập.", null);
        }

        if (string.IsNullOrWhiteSpace(request.Name))
        {
            return (false, "Tên tài khoản không được để trống.", null);
        }

        if (string.IsNullOrWhiteSpace(request.Email))
        {
            return (false, "Email không được để trống.", null);
        }

        if (request.GroupPermissionId == Guid.Empty || !await _repository.GroupPermissionExistsAsync(request.GroupPermissionId, cancellationToken))
        {
            return (false, "Nhóm quyền truy cập không hợp lệ.", null);
        }

        existing.Name = request.Name.Trim();
        existing.Email = request.Email.Trim();
        existing.Status = NormalizeStatus(request.Status);
        existing.Content = NormalizeContent(request.Content);
        existing.GroupPermissionId = request.GroupPermissionId;

        await _repository.UpdateAsync(existing, string.IsNullOrWhiteSpace(request.Password) ? null : request.Password.Trim(), cancellationToken);
        var updated = await _repository.GetByIdAsync(id, cancellationToken);
        return (true, "Cập nhật tài khoản truy cập thành công.", updated?.ToDto());
    }

    public async Task<(bool IsSuccess, string Message)> ResetPasswordAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var existing = await _repository.GetByIdAsync(id, cancellationToken);
        if (existing == null)
        {
            return (false, "Không tìm thấy tài khoản truy cập.");
        }

        await _repository.ResetPasswordAsync(id, cancellationToken);
        return (true, "Đặt lại mật khẩu mặc định thành công.");
    }

    public async Task<(bool IsSuccess, string Message)> ChangeStatusAsync(Guid id, string status, CancellationToken cancellationToken = default)
    {
        var existing = await _repository.GetByIdAsync(id, cancellationToken);
        if (existing == null)
        {
            return (false, "Không tìm thấy tài khoản truy cập.");
        }

        await _repository.ChangeStatusAsync(id, NormalizeStatus(status), cancellationToken);
        return (true, "Cập nhật trạng thái tài khoản thành công.");
    }

    public async Task<(bool IsSuccess, string Message, UserAccountDto? Data)> DuplicateAsync(DuplicateUserAccountRequest request, CancellationToken cancellationToken = default)
    {
        if (request.SourceUserId == Guid.Empty)
        {
            return (false, "Tài khoản nguồn không hợp lệ.", null);
        }

        if (string.IsNullOrWhiteSpace(request.Username) || string.IsNullOrWhiteSpace(request.Name) || string.IsNullOrWhiteSpace(request.Email))
        {
            return (false, "Thông tin nhân bản không được để trống.", null);
        }

        var source = await _repository.GetByIdAsync(request.SourceUserId, cancellationToken);
        if (source == null)
        {
            return (false, "Không tìm thấy tài khoản nguồn để nhân bản.", null);
        }

        if (await _repository.ExistsByUsernameOrEmailAsync(request.Username.Trim(), request.Email.Trim(), null, cancellationToken))
        {
            return (false, "Username hoặc email đã tồn tại trong hệ thống.", null);
        }

        await _repository.DuplicateAsync(request.SourceUserId, request.Username.Trim(), request.Name.Trim(), request.Email.Trim(), cancellationToken);
        var duplicated = await _repository.GetPagedAsync(request.Username.Trim(), 5, 1, null, cancellationToken);
        var created = duplicated.Items.FirstOrDefault(x => string.Equals(x.Username, request.Username.Trim(), StringComparison.OrdinalIgnoreCase));
        return (true, "Nhân bản tài khoản truy cập thành công.", created?.ToDto());
    }

    public async Task<(bool IsSuccess, string Message)> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var existing = await _repository.GetByIdAsync(id, cancellationToken);
        if (existing == null)
        {
            return (false, "Không tìm thấy tài khoản truy cập.");
        }

        await _repository.DeleteAsync(id, cancellationToken);
        return (true, "Xóa tài khoản truy cập thành công.");
    }

    private static string NormalizeStatus(string? status)
    {
        return status?.Trim() switch
        {
            "Chờ kích hoạt" => "Chờ kích hoạt",
            "Khóa" => "Khóa",
            _ => "Kích hoạt"
        };
    }

    private static string NormalizeContent(string? content)
    {
        return string.Equals(content?.Trim(), "Max", StringComparison.OrdinalIgnoreCase) ? "Max" : "Fixted";
    }
}

