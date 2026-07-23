using AEspejo.Clinic.Infrastructure.MultiTenancy;
using AEspejo.Clinic.Master;
using Microsoft.EntityFrameworkCore;

namespace AEspejo.Clinic.API.Middleware;

/// <summary>
/// Resolves the request's tenant from the subdomain (e.g. demo.localhost) or the X-Tenant header,
/// queries the master database and populates the scoped TenantContext. No valid tenant → 400.
/// Public routes (auth, documentation, health checks) are skipped.
/// </summary>
public class TenantResolutionMiddleware
{
    private readonly RequestDelegate _next;

    private static readonly string[] PublicPrefixes =
        ["/scalar", "/openapi", "/health", "/favicon"];

    public TenantResolutionMiddleware(RequestDelegate next) => _next = next;

    public async Task InvokeAsync(HttpContext context, TenantContext tenantContext, MasterDbContext master)
    {
        var path = context.Request.Path.Value ?? string.Empty;
        if (PublicPrefixes.Any(p => path.StartsWith(p, StringComparison.OrdinalIgnoreCase)))
        {
            await _next(context);
            return;
        }

        var identifier = ResolveIdentifier(context);
        if (string.IsNullOrWhiteSpace(identifier))
        {
            await WriteProblem(context, StatusCodes.Status400BadRequest,
                "Falta el tenant. Use el subdominio (empresa.host) o el header 'X-Tenant'.");
            return;
        }

        var tenant = await master.Tenants
            .AsNoTracking()
            .FirstOrDefaultAsync(t => t.Subdomain == identifier && t.IsActive);

        if (tenant is null)
        {
            await WriteProblem(context, StatusCodes.Status404NotFound,
                $"Tenant '{identifier}' no encontrado o inactivo.");
            return;
        }

        tenantContext.SetTenant(tenant.Subdomain, tenant.ConnectionString, tenant.DefaultLanguage);
        await _next(context);
    }

    private static string? ResolveIdentifier(HttpContext context)
    {
        // 1) Explicit header (handy for local dev): X-Tenant
        if (context.Request.Headers.TryGetValue("X-Tenant", out var header) && !string.IsNullOrWhiteSpace(header))
            return header.ToString().Trim().ToLowerInvariant();

        // 2) Subdomain: first host segment when there are at least 2 dots (sub.domain.tld) or *.localhost
        var host = context.Request.Host.Host;
        var parts = host.Split('.');
        if (parts.Length >= 2 && !string.Equals(parts[0], "www", StringComparison.OrdinalIgnoreCase))
            return parts[0].ToLowerInvariant();

        return null;
    }

    private static async Task WriteProblem(HttpContext context, int status, string detail)
    {
        context.Response.StatusCode = status;
        context.Response.ContentType = "application/problem+json";
        await context.Response.WriteAsJsonAsync(new { status, detail });
    }
}
