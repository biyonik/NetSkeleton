using Application.Common.Constants;
using Application.Common.Security.Attributes;
using Application.Users.Commands;
using Application.Users.Queries;
using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers.V1;

[ApiVersion("1.0")]
public class UsersController : BaseApiController
{
    [HttpGet]
    [RequirePermission(SystemPermissions.UsersView)]
    public async Task<IActionResult> GetUsers([FromQuery] GetUsersQuery query)
    {
        var result = await Mediator.Send(query);
        return FromResult(result);
    }

    [HttpGet("{id}")]
    [RequirePermission(SystemPermissions.UsersView)]
    public async Task<IActionResult> GetUser(Guid id)
    {
        var result = await Mediator.Send(new GetUserQuery(id));
        return FromResult(result);
    }

    [HttpPost]
    [RequirePermission(SystemPermissions.UsersCreate)]
    public async Task<IActionResult> CreateUser([FromBody] CreateUserCommand command)
    {
        var result = await Mediator.Send(command);
        return FromResult(result);
    }

    [HttpPut("{id}")]
    [RequirePermission(SystemPermissions.UsersEdit)]
    public async Task<IActionResult> UpdateUser(Guid id, [FromBody] UpdateUserCommand command)
    {
        if (id != command.Id)
            return BadRequest();

        var result = await Mediator.Send(command);
        return FromResult(result);
    }

    [HttpDelete("{id}")]
    [RequirePermission(SystemPermissions.UsersEdit)]
    public async Task<IActionResult> DeleteUser(Guid id)
    {
        var result = await Mediator.Send(new DeleteUserCommand(id));
        return FromResult(result);
    }

    [HttpPost("{id}/roles")]
    [RequirePermission(SystemPermissions.UsersAssignRoles)]
    public async Task<IActionResult> AssignRoles(Guid id, [FromBody] AssignUserRolesCommand command)
    {
        if (id != command.UserId)
            return BadRequest();

        var result = await Mediator.Send(command);
        return FromResult(result);
    }

    [HttpPost("{id}/permissions")]
    [RequirePermission(SystemPermissions.UsersAssignPermissions)]
    public async Task<IActionResult> AssignPermissions(Guid id, [FromBody] AssignUserPermissionsCommand command)
    {
        if (id != command.UserId)
            return BadRequest();

        var result = await Mediator.Send(command);
        return FromResult(result);
    }
}