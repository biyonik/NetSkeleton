using Application.Common.Security.Attributes;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace API.Filters;

/// <summary>
/// Swagger için permission'ları dokümante eden filter
/// </summary>
public class PermissionOperationFilter : IOperationFilter
{
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        var actionMetadata = context.ApiDescription.ActionDescriptor.EndpointMetadata;
        
        // Permission attribute'larını bul
        var permissionAttributes = actionMetadata
            .OfType<RequirePermissionAttribute>()
            .ToList();

        // Resource authorization attribute'larını bul
        var resourceAttributes = actionMetadata
            .OfType<ResourceAuthorizationAttribute>()
            .ToList();

        if (!permissionAttributes.Any() && !resourceAttributes.Any())
            return;

        // Security requirement ekle
        operation.Security = new List<OpenApiSecurityRequirement>
        {
            new()
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "Bearer"
                        }
                    },
                    new List<string>()
                }
            }
        };

        // Permission'ları description'a ekle
        var requirements = new List<string>();

        if (permissionAttributes.Any())
        {
            var permissions = permissionAttributes
                .SelectMany(attr => attr.ToString().Split('_')[1].Split(','))
                .ToList();
            
            requirements.Add($"Required Permissions: {string.Join(", ", permissions)}");
        }

        if (resourceAttributes.Any())
        {
            foreach (var attr in resourceAttributes)
            {
                var parts = attr.ToString().Split('_');
                requirements.Add($"Required Resource Access: {parts[1]} - Operation: {parts[2]}");
            }
        }

        operation.Description = string.Join("\n", requirements) + 
            (string.IsNullOrEmpty(operation.Description) 
                ? "" 
                : $"\n\n{operation.Description}");
    }
}