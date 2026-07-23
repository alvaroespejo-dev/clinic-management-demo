using AEspejo.Clinic.Application.Interfaces;

namespace AEspejo.Clinic.Infrastructure.MultiTenancy;

/// <summary>
/// Scoped carrier of the tenant resolved for the request. The middleware populates it, the
/// <c>AppDbContext</c> consumes it (via <see cref="ITenantConnectionResolver"/>) and services
/// read the default language (via <see cref="ITenantProvider"/>).
/// </summary>
public class TenantContext : ITenantProvider, ITenantConnectionResolver
{
    public string? TenantIdentifier { get; private set; }
    public string? ConnectionString { get; private set; }
    public string? DefaultLanguage { get; private set; }
    public bool IsResolved { get; private set; }

    public void SetTenant(string identifier, string connectionString, string defaultLanguage)
    {
        TenantIdentifier = identifier;
        ConnectionString = connectionString;
        DefaultLanguage = defaultLanguage;
        IsResolved = true;
    }

    public string? GetConnectionString() => ConnectionString;
}
