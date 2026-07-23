using AEspejo.Clinic.Domain.Common;

namespace AEspejo.Clinic.Domain.Entities;

/// <summary>
/// Clinical data of a professional/dentist. 1:1 relationship with User (Id = UserId).
/// </summary>
public class Professional : BaseEntity
{
    // Id (inherited from BaseEntity) is the PK shared with User (1:1); it is set to UserId on create.
    public User User { get; set; } = null!;

    public string LicenseNumber { get; set; } = string.Empty;
    public string Specialty { get; set; } = string.Empty;
    /// <summary>Color used to represent the professional in the UI calendar.</summary>
    public string Color { get; set; } = "#3b82f6";

    public ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();
    public ICollection<TreatmentPlan> TreatmentPlans { get; set; } = new List<TreatmentPlan>();
}
