namespace BuildingBlocks.Abstractions;

public interface ICurrentUserContext
{
    Guid? UserId { get; }
    string? Username { get; }
    Guid? DonViId { get; }
    Guid? GroupPermissionId { get; }
    bool IsSSA { get; }
    bool IsAuthenticated { get; }
}
