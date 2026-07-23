namespace AEspejo.Clinic.Infrastructure.MultiTenancy;

/// <summary>
/// Resolves the connection string of the active tenant's database.
/// The concrete implementation (queries the master database using the subdomain)
/// is registered in the API layer.
/// </summary>
public interface ITenantConnectionResolver
{
    /// <summary>Returns the current tenant's connection string, or null when no tenant is resolved.</summary>
    string? GetConnectionString();
}
