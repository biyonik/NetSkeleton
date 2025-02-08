using Microsoft.AspNetCore.Authorization;

namespace Application.Common.Security.Attributes;

/// <summary>
/// Sistem admin kontrolü yapan attribute
/// </summary>
public class SystemAdminAttribute() : AuthorizeAttribute("SystemAdmin");