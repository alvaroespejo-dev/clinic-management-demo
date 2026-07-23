using AEspejo.Clinic.Domain.Common;
using AEspejo.Clinic.Domain.Enums;

namespace AEspejo.Clinic.Domain.Entities;

/// <summary>Budgeted, multi-appointment treatment plan for a patient.</summary>
public class TreatmentPlan : BaseEntity
{
    public Guid PatientId { get; set; }
    public Patient Patient { get; set; } = null!;

    public Guid ProfessionalId { get; set; }
    public Professional Professional { get; set; } = null!;

    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public TreatmentPlanStatus Status { get; set; } = TreatmentPlanStatus.Draft;

    public ICollection<TreatmentPlanItem> Items { get; set; } = new List<TreatmentPlanItem>();
}
