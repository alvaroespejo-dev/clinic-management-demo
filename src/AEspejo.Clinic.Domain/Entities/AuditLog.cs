using AEspejo.Clinic.Domain.Enums;

namespace AEspejo.Clinic.Domain.Entities;

/// <summary>
/// Audit trail. Written automatically by the EF Core interceptor
/// on every Created/Updated/Deleted. Does not inherit from BaseEntity (auto-increment long PK).
/// </summary>
public class AuditLog
{
    public long Id { get; set; }
    public string TableName { get; set; } = string.Empty;
    public string RecordId { get; set; } = string.Empty;
    public AuditAction Action { get; set; }
    public string? OldValues { get; set; }
    public string? NewValues { get; set; }
    public Guid? UserId { get; set; }
    public DateTimeOffset ChangedAt { get; set; }
    public string? IpAddress { get; set; }
}
