using AEspejo.Clinic.Domain.Common;

namespace AEspejo.Clinic.Domain.Entities;

/// <summary>
/// Translation of a catalog service into a specific language.
/// Unique index on (ServiceId, LanguageCode).
/// </summary>
public class ServiceCatalogTranslation : BaseEntity
{
    public Guid ServiceId { get; set; }
    public ServiceCatalog Service { get; set; } = null!;

    /// <summary>ISO 639-1 language code, e.g. "es", "en".</summary>
    public string LanguageCode { get; set; } = string.Empty;

    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string Category { get; set; } = string.Empty;
}
