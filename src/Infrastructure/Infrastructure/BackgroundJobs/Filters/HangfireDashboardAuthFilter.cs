using System.Text;
using Hangfire.Dashboard;
using Infrastructure.BackgroundJobs.Settings;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;

namespace Infrastructure.BackgroundJobs.Filters;

/// <summary>
/// Hangfire dashboard için basic authentication filter
/// </summary>
public class HangfireDashboardAuthFilter(IOptions<BackgroundJobSettings> settings) : IDashboardAuthorizationFilter
{
    private readonly string _username = settings.Value.DashboardUsername;
    private readonly string _password = settings.Value.DashboardPassword;

    public bool Authorize(DashboardContext context)
    {
        var httpContext = context.GetHttpContext();

        // Basic authentication header'ı al
        string header = httpContext.Request.Headers["Authorization"].ToString();
        
        if (string.IsNullOrWhiteSpace(header) || !header.StartsWith("Basic "))
        {
            SetUnauthorizedResponse(httpContext);
            return false;
        }

        // Header'ı decode et
        var credentials = GetCredentialsFromHeader(header);
        if (credentials == null)
        {
            SetUnauthorizedResponse(httpContext);
            return false;
        }

        // Kullanıcı adı ve şifreyi kontrol et
        if (credentials[0] == _username && credentials[1] == _password)
            return true;

        SetUnauthorizedResponse(httpContext);
        return false;
    }

    private string[]? GetCredentialsFromHeader(string header)
    {
        try
        {
            var encodedCredentials = header.Substring("Basic ".Length).Trim();
            var decodedCredentials = Encoding.UTF8.GetString(Convert.FromBase64String(encodedCredentials));
            return decodedCredentials.Split(':', 2);
        }
        catch
        {
            return null;
        }
    }

    private void SetUnauthorizedResponse(HttpContext httpContext)
    {
        httpContext.Response.Headers["WWW-Authenticate"] = "Basic realm=\"Hangfire Dashboard\"";
        httpContext.Response.StatusCode = 401;
    }
}