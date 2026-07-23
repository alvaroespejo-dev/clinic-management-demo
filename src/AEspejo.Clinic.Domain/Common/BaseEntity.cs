namespace AEspejo.Clinic.Domain.Common;

/// <summary>
/// Base class for all business entities, with a Guid identity and audit fields.
/// </summary>
public abstract class BaseEntity
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset? UpdatedAt { get; set; }
}
