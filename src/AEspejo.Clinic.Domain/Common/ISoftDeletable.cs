namespace AEspejo.Clinic.Domain.Common;

/// <summary>
/// Marks entities that are "deleted" logically (IsActive = false) instead of physically.
/// The generic CRUD detects it so clinical records are never physically removed.
/// </summary>
public interface ISoftDeletable
{
    bool IsActive { get; set; }
}
