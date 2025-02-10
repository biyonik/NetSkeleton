using Application.Common.Constants;
using Application.Common.Security.Attributes;
using Application.Permissions.Commands;
using Application.Permissions.Queries;
using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers.V1;

[ApiVersion("1.0")]
public class PermissionsController : BaseApiController
{
   /// <summary>
   /// Tüm permission'ları listeler
   /// </summary>
   [HttpGet]
   [RequirePermission(SystemPermissions.PermissionsView)]
   public async Task<IActionResult> GetPermissions([FromQuery] GetPermissionsQuery query)
   {
       var result = await Mediator.Send(query);
       return FromResult(result);
   }

   /// <summary>
   /// ID'ye göre permission getirir
   /// </summary>
   [HttpGet("{id}")]
   [RequirePermission(SystemPermissions.PermissionsView)]
   public async Task<IActionResult> GetPermission(Guid id)
   {
       var result = await Mediator.Send(new GetPermissionQuery(id));
       return FromResult(result);
   }

   /// <summary>
   /// Permission kategorilerini listeler
   /// </summary>
   [HttpGet("categories")]
   [RequirePermission(SystemPermissions.PermissionsView)]
   public async Task<IActionResult> GetCategories()
   {
       var result = await Mediator.Send(new GetPermissionCategoriesQuery());
       return FromResult(result);
   }

   /// <summary>
   /// Yeni permission oluşturur
   /// </summary>
   [HttpPost]
   [RequirePermission(SystemPermissions.PermissionsCreate)]
   public async Task<IActionResult> CreatePermission([FromBody] CreatePermissionCommand command)
   {
       var result = await Mediator.Send(command);
       return FromResult(result);
   }

   /// <summary>
   /// Permission bilgilerini günceller
   /// </summary>
   [HttpPut("{id}")]
   [RequirePermission(SystemPermissions.PermissionsEdit)]
   public async Task<IActionResult> UpdatePermission(Guid id, [FromBody] UpdatePermissionCommand command)
   {
       if (id != command.Id)
           return BadRequest();

       var result = await Mediator.Send(command);
       return FromResult(result);
   }

   /// <summary>
   /// Permission siler
   /// </summary>
   [HttpDelete("{id}")]
   [RequirePermission(SystemPermissions.PermissionsDelete)]
   public async Task<IActionResult> DeletePermission(Guid id)
   {
       var result = await Mediator.Send(new DeletePermissionCommand(id));
       return FromResult(result);
   }
   
   /// <summary>
   /// Permission'a endpoint atar
   /// </summary>
   [HttpPost("{id}/endpoints")]
   [RequirePermission(SystemPermissions.PermissionsEdit)]
   public async Task<IActionResult> AssignEndpoint(Guid id, [FromBody] AssignEndpointToPermissionCommand command)
   {
       if (id != command.PermissionId)
           return BadRequest();

       var result = await Mediator.Send(command);
       return FromResult(result);
   }

   /// <summary>
   /// Permission'dan endpoint kaldırır
   /// </summary>
   [HttpDelete("endpoints/{endpointId}")]
   [RequirePermission(SystemPermissions.PermissionsEdit)]
   public async Task<IActionResult> RemoveEndpoint(Guid endpointId)
   {
       var result = await Mediator.Send(new RemoveEndpointFromPermissionCommand(endpointId));
       return FromResult(result);
   }

   /// <summary>
   /// Permission'a ait endpointleri getirir
   /// </summary>
   [HttpGet("{id}/endpoints")]
   [RequirePermission(SystemPermissions.PermissionsView)]
   public async Task<IActionResult> GetEndpoints(Guid id)
   {
       var result = await Mediator.Send(new GetPermissionEndpointsQuery(id));
       return FromResult(result);
   }

   /// <summary>
   /// Endpoint'e ait permissionları getirir
   /// </summary>
   [HttpGet("endpoints")]
   [RequirePermission(SystemPermissions.PermissionsView)]
   public async Task<IActionResult> GetEndpointPermissions([FromQuery] GetEndpointPermissionsQuery query)
   {
       var result = await Mediator.Send(query);
       return FromResult(result);
   }
}