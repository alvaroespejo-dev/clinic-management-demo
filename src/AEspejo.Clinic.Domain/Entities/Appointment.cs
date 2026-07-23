using AEspejo.Clinic.Domain.Common;
using AEspejo.Clinic.Domain.Enums;

namespace AEspejo.Clinic.Domain.Entities;

/// <summary>Appointment scheduled for a patient with a professional.</summary>
public class Appointment : BaseEntity
{
    public Guid BranchId { get; set; }
    public Branch Branch { get; set; } = null!;

    public Guid PatientId { get; set; }
    public Patient Patient { get; set; } = null!;

    public Guid ProfessionalId { get; set; }
    public Professional Professional { get; set; } = null!;

    public Guid? RoomId { get; set; }
    public Room? Room { get; set; }

    public DateTimeOffset ScheduledAt { get; set; }
    public int DurationMinutes { get; set; }
    public AppointmentStatus Status { get; set; } = AppointmentStatus.Scheduled;
    public string? Notes { get; set; }
    public string? CancelReason { get; set; }

    public Guid CreatedByUserId { get; set; }
    public User CreatedByUser { get; set; } = null!;

    public ICollection<AppointmentDetail> Details { get; set; } = new List<AppointmentDetail>();
}
