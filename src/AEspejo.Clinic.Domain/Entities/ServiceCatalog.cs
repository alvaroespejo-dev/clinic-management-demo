using AEspejo.Clinic.Domain.Common;

namespace AEspejo.Clinic.Domain.Entities;

/// <summary>
/// Catalog of procedures/services. Non-translatable fields only;
/// the visible text (name, description, category) lives in <see cref="ServiceCatalogTranslation"/>.
/// </summary>
public class ServiceCatalog : BaseEntity, ISoftDeletable
{
    /// <summary>Internal code, e.g. "EXT-01".</summary>
    public string Code { get; set; } = string.Empty;
    public decimal DefaultPrice { get; set; }
    public bool IsActive { get; set; } = true;

    public ICollection<ServiceCatalogTranslation> Translations { get; set; } = new List<ServiceCatalogTranslation>();
}
