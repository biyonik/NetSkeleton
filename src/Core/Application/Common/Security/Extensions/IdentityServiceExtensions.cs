using System.Reflection;
using System.Security.Claims;
using System.Text;
using Application.Common.Security.Abstractions;
using Application.Common.Security.Authorization;
using Application.Common.Security.Handlers;
using Application.Common.Security.Services;
using Application.Common.Security.Settings;
using Domain.Security;
using Infrastructure.Identity.Models;
using Infrastructure.Identity.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Persistence.Context;

namespace Application.Common.Security.Extensions;

/// <summary>
/// Identity servislerinin registrasyonu için extension methods
/// </summary>
public static class IdentityServiceExtensions
{
    /// <summary>
    /// Identity ve JWT servislerini register eder
    /// </summary>
    public static IServiceCollection AddIdentityServices(
        this IServiceCollection services, 
        IConfiguration configuration)
    {
        // JWT ayarlarını register et
        var jwtSettings = new JwtSettings();
        configuration.GetSection("JwtSettings").Bind(jwtSettings);
        services.Configure<JwtSettings>(configuration.GetSection("JwtSettings"));

        // Identity ayarlarını yapılandır
        services.AddIdentity<ApplicationUser, ApplicationRole>(options =>
        {
            // Password ayarları
            options.Password.RequiredLength = 8;
            options.Password.RequireDigit = true;
            options.Password.RequireLowercase = true;
            options.Password.RequireUppercase = true;
            options.Password.RequireNonAlphanumeric = true;

            // Lockout ayarları
            options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(15);
            options.Lockout.MaxFailedAccessAttempts = 5;

            // User ayarları
            options.User.RequireUniqueEmail = true;
            options.SignIn.RequireConfirmedEmail = true;
        })
        .AddEntityFrameworkStores<ApplicationDbContext>()
        .AddDefaultTokenProviders();

        // JWT authentication ayarlarını yapılandır
        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ClockSkew = TimeSpan.Zero,
                ValidIssuer = jwtSettings.Issuer,
                ValidAudience = jwtSettings.Audience,
                IssuerSigningKey = new SymmetricSecurityKey(
                    Encoding.UTF8.GetBytes(jwtSettings.Secret))
            };

            // Token doğrulama hatalarını logla
            options.Events = new JwtBearerEvents
            {
                OnAuthenticationFailed = context =>
                {
                    if (context.Exception.GetType() == typeof(SecurityTokenExpiredException))
                    {
                        context.Response.Headers.Add("Token-Expired", "true");
                    }
                    return Task.CompletedTask;
                }
            };
        });

        // Identity servisleri
        services.AddScoped<IJwtService, JwtService>();
        services.AddScoped<ICurrentUserService, CurrentUserService>();
        services.AddScoped<IDynamicPolicyService, DynamicPolicyService>();

        // Authorization Handlers
        services.AddScoped<IAuthorizationHandler, PermissionAuthorizationHandler>();
        services.AddScoped<IAuthorizationHandler, ResourceAuthorizationHandler>();
        services.AddScoped<IAuthorizationHandler, DynamicPolicyHandler>();

        // Authorization Policies
        services.AddAuthorization(options =>
        {
            // Super Admin policy
            options.AddPolicy("SuperAdmin", policy =>
                policy.RequireRole("SuperAdmin"));

            // System Admin policy
            options.AddPolicy("SystemAdmin", policy =>
                policy.RequireRole("SystemAdmin", "SuperAdmin"));

            // Permission policies
            var permissions = typeof(Permission)
                .GetFields(BindingFlags.Public | BindingFlags.Static)
                .Where(f => f.FieldType == typeof(Permission))
                .Select(f => (Permission)f.GetValue(null)!)
                .ToList();

            foreach (var permission in permissions)
            {
                options.AddPolicy($"Permission_{permission.Code}", policy =>
                    policy.Requirements.Add(new PermissionRequirement(new[] { permission.Code })));
            }
        });

        return services;
    }

    /// <summary>
    /// Admin kullanıcısını ve rolleri oluşturur
    /// </summary>
    public static async Task SeedIdentityDataAsync(IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
        var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<ApplicationRole>>();

        // Rolleri oluştur
        var roles = new[] { "SuperAdmin", "SystemAdmin", "TenantAdmin", "User" };
        foreach (var role in roles)
        {
            if (!await roleManager.RoleExistsAsync(role))
            {
                await roleManager.CreateAsync(new ApplicationRole { Name = role });
            }
        }

        // Super Admin kullanıcısını oluştur
        var superAdminEmail = "admin@example.com";
        var superAdmin = await userManager.FindByEmailAsync(superAdminEmail);

        if (superAdmin == null)
        {
            superAdmin = new ApplicationUser
            {
                UserName = superAdminEmail,
                Email = superAdminEmail,
                EmailConfirmed = true,
                FirstName = "Super",
                LastName = "Admin",
                IsActive = true
            };

            var result = await userManager.CreateAsync(superAdmin, "Admin123!");
            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(superAdmin, "SuperAdmin");

                // Tüm permission'ları ekle
                var permissions = typeof(Permission)
                    .GetFields(BindingFlags.Public | BindingFlags.Static)
                    .Where(f => f.FieldType == typeof(Permission))
                    .Select(f => (Permission)f.GetValue(null)!)
                    .ToList();

                foreach (var permission in permissions)
                {
                    await userManager.AddClaimAsync(superAdmin, 
                        new Claim("Permission", permission.Code));
                }
            }
        }
    }
}