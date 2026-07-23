namespace AEspejo.Clinic.Master.Entities;

/// <summary>
/// Language supported by the system. Catalog stored in the master database; adding a
/// future language means inserting a row, not changing the schema.
/// </summary>
public class Language
{
    /// <summary>PK — ISO 639-1 code, e.g. "es", "en".</summary>
    public string Code { get; set; } = string.Empty;

    /// <summary>Display name, e.g. "Español", "English".</summary>
    public string Name { get; set; } = string.Empty;

    public bool IsActive { get; set; } = true;
}
