namespace Application.Permissions.Dtos;

public class PermissionEndpointDto
{
    public Guid Id { get; set; }
    public Guid PermissionId { get; set; }
    public string Controller { get; set; } = null!;
    public string Action { get; set; } = null!;
    public string HttpMethod { get; set; } = null!;
    public string Route { get; set; } = null!;
    public DateTime CreatedDate { get; set; }
    public Guid? CreatedBy { get; set; }
}