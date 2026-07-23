namespace AEspejo.Clinic.Domain.Entities;

/// <summary>
/// Organization configuration entity that stores clinic name, logo, and other global settings.
/// </summary>
public class OrgConfig
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? LogoUrl { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
