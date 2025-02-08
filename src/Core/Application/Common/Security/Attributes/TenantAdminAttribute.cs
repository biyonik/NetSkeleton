using Microsoft.AspNetCore.Authorization;

namespace Application.Common.Security.Attributes;

/// <summary>
/// Tenant admin kontrolü yapan attribute
/// </summary>
public class TenantAdminAttribute() : AuthorizeAttribute("TenantAdmin");