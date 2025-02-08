using Microsoft.AspNetCore.Authorization;

namespace Application.Common.Security.Attributes;

/// <summary>
/// Super admin kontrolü yapan attribute
/// </summary>
public class SuperAdminAttribute() : AuthorizeAttribute("SuperAdmin");