using AEspejo.Clinic.Domain.Common;

namespace AEspejo.Clinic.Domain.Entities;

/// <summary>Relevant medical condition of a patient (diabetes, hypertension, etc.).</summary>
public class MedicalCondition : BaseEntity, ISoftDeletable
{
    public Guid PatientId { get; set; }
    public Patient Patient { get; set; } = null!;

    public string Name { get; set; } = string.Empty;
    public string? Notes { get; set; }
    public bool IsActive { get; set; } = true;
}
