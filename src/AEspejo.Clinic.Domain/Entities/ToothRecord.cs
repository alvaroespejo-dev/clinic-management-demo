using AEspejo.Clinic.Domain.Common;
using AEspejo.Clinic.Domain.Enums;

namespace AEspejo.Clinic.Domain.Entities;

/// <summary>State of a tooth (or surface) within an odontogram.</summary>
public class ToothRecord : BaseEntity
{
    public Guid OdontogramId { get; set; }
    public Odontogram Odontogram { get; set; } = null!;

    /// <summary>Tooth number in FDI notation (11-48).</summary>
    public int ToothNumber { get; set; }
    public ToothSurface Surface { get; set; }
    public ToothStatus Status { get; set; }
    public string? Notes { get; set; }

    /// <summary>User who recorded the last state change.</summary>
    public Guid UpdatedByUserId { get; set; }
    public User UpdatedByUser { get; set; } = null!;
}
