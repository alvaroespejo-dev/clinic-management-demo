using AEspejo.Clinic.Domain.Common;

namespace AEspejo.Clinic.Domain.Entities;

/// <summary>Specific procedure performed/recorded during an appointment.</summary>
public class AppointmentDetail : BaseEntity
{
    public Guid AppointmentId { get; set; }
    public Appointment Appointment { get; set; } = null!;

    public Guid ServiceId { get; set; }
    public ServiceCatalog Service { get; set; } = null!;

    /// <summary>Affected tooth (FDI notation), if applicable.</summary>
    public int? ToothNumber { get; set; }
    public int Quantity { get; set; } = 1;

    /// <summary>Price frozen at the time of the appointment.</summary>
    public decimal UnitPrice { get; set; }
    public string? Notes { get; set; }
}
