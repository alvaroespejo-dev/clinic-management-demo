using AEspejo.Clinic.Master.Enums;

namespace AEspejo.Clinic.Master.Entities;

/// <summary>
/// Client company/clinic of the software (tenant). Lives in the master database and points
/// to that company's own database.
/// </summary>
public class Tenant
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public string CompanyName { get; set; } = string.Empty;

    /// <summary>Unique subdomain, e.g. "smileclinic" → smileclinic.aespejo.com.</summary>
    public string Subdomain { get; set; } = string.Empty;

    /// <summary>Company database name, e.g. "Clinic_SmileClinic".</summary>
    public string DatabaseName { get; set; } = string.Empty;

    /// <summary>Connection string to the company database (encrypted at rest).</summary>
    public string ConnectionString { get; set; } = string.Empty;

    public TenantPlan Plan { get; set; } = TenantPlan.Trial;

    /// <summary>Company default language (ISO 639-1), e.g. "es".</summary>
    public string DefaultLanguage { get; set; } = "es";

    public string ContactEmail { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;

    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset? UpdatedAt { get; set; }
}
