namespace AEspejo.Clinic.Application.Interfaces;

/// <summary>
/// Exposes the tenant (company) active in the current request and its connection string.
/// The concrete implementation (middleware resolving by subdomain + master database)
/// lives in the API/Infrastructure layer.
/// </summary>
public interface ITenantProvider
{
    /// <summary>Current tenant subdomain/identifier, e.g. "smileclinic".</summary>
    string? TenantIdentifier { get; }

    /// <summary>Connection string to the current tenant's database.</summary>
    string? ConnectionString { get; }

    /// <summary>Tenant default language (ISO 639-1), e.g. "es".</summary>
    string? DefaultLanguage { get; }
}
