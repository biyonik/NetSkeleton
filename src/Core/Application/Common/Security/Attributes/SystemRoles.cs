namespace Application.Common.Security.Attributes;

/// <summary>
/// Sistem rollerine göre yetkilendirme için attribute
/// </summary>
public static class SystemRoles
{
    public class SuperAdminAttribute() : RequirePermissionAttribute("System.SuperAdmin");
    public class SystemAdminAttribute() : RequirePermissionAttribute("System.Admin");
    public class TenantAdminAttribute() : RequirePermissionAttribute("Tenant.Admin");
}