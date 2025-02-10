namespace Application.Permissions.Dtos;

public class PermissionDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = null!;
    public string SystemName { get; set; } = null!;
    public string? Description { get; set; }
    public string? Category { get; set; }
    public bool IsSystemPermission { get; set; }
    public string? RequiredClaims { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedDate { get; set; }
    public Guid? CreatedBy { get; set; }
    public DateTime? LastModifiedDate { get; set; }
    public Guid? LastModifiedBy { get; set; }
}