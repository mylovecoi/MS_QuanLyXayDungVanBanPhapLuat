using System.ComponentModel.DataAnnotations;

namespace QuanTriHeThongService.Contracts.Requests.Internal;

public sealed class PermissionCheckApiRequest
{
    [Required]
    public string Controller { get; init; } = string.Empty;

    [Required]
    public string Action { get; init; } = string.Empty;

    [Required]
    public string PermissionType { get; init; } = string.Empty;
}
