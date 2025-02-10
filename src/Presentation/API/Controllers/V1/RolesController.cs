using Application.Common.Constants;
using Application.Common.Security.Attributes;
using Application.Roles.Commands;
using Application.Roles.Queries;
using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers.V1;

[ApiVersion("1.0")]
public class RolesController : BaseApiController
{
    /// <summary>
    /// Tüm rolleri listeler
    /// </summary>
    [HttpGet]
    [RequirePermission(SystemPermissions.RolesView)]
    public async Task<IActionResult> GetRoles([FromQuery] GetRolesQuery query)
    {
        var result = await Mediator.Send(query);
        return FromResult(result);
    }

    /// <summary>
    /// ID'ye göre rol getirir
    /// </summary>
    [HttpGet("{id}")]
    [RequirePermission(SystemPermissions.RolesView)]
    public async Task<IActionResult> GetRole(Guid id)
    {
        var result = await Mediator.Send(new GetRoleQuery(id));
        return FromResult(result);
    }

    /// <summary>
    /// Yeni rol oluşturur
    /// </summary>
    [HttpPost]
    [RequirePermission(SystemPermissions.RolesCreate)]
    public async Task<IActionResult> CreateRole([FromBody] CreateRoleCommand command)
    {
        var result = await Mediator.Send(command);
        return FromResult(result);
    }

    /// <summary>
    /// Rol bilgilerini günceller
    /// </summary>
    [HttpPut("{id}")]
    [RequirePermission(SystemPermissions.RolesEdit)]
    public async Task<IActionResult> UpdateRole(Guid id, [FromBody] UpdateRoleCommand command)
    {
        if (id != command.Id)
            return BadRequest();

        var result = await Mediator.Send(command);
        return FromResult(result);
    }

    /// <summary>
    /// Rol siler
    /// </summary>
    [HttpDelete("{id}")]
    [RequirePermission(SystemPermissions.RolesDelete)]
    public async Task<IActionResult> DeleteRole(Guid id)
    {
        var result = await Mediator.Send(new DeleteRoleCommand(id));
        return FromResult(result);
    }

    /// <summary>
    /// Role permission'lar atar
    /// </summary>
    [HttpPost("{id}/permissions")]
    [RequirePermission(SystemPermissions.RolesAssignPermissions)]
    public async Task<IActionResult> AssignPermissions(Guid id, [FromBody] AssignRolePermissionsCommand command)
    {
        if (id != command.RoleId)
            return BadRequest();

        var result = await Mediator.Send(command);
        return FromResult(result);
    }
}