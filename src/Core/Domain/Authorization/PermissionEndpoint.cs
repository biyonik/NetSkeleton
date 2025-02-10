using Domain.Common.Abstractions;

namespace Domain.Authorization;

public class PermissionEndpoint : BaseEntity<Guid>
{
    public Guid PermissionId { get; private set; }
    public Permission Permission { get; private set; } = null!;
    public string Controller { get; private set; } = null!;
    public string Action { get; private set; } = null!;
    public string HttpMethod { get; private set; } = null!;
    public string Route { get; private set; } = null!;
    public DateTime CreatedDate { get; private set; } = DateTime.UtcNow;
    public Guid? CreatedBy { get; private set; }

    protected PermissionEndpoint() { } // EF Core için

    public static PermissionEndpoint Create(
        Guid permissionId,
        string controller,
        string action,
        string httpMethod,
        string route)
    {
        return new PermissionEndpoint
        {
            Id = Guid.NewGuid(),
            PermissionId = permissionId,
            Controller = controller,
            Action = action,
            HttpMethod = httpMethod,
            Route = route
        };
    }
}