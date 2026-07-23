using AEspejo.Clinic.Domain.Common;
using AEspejo.Clinic.Domain.Enums;

namespace AEspejo.Clinic.Domain.Entities;

/// <summary>Allergy recorded for a patient.</summary>
public class PatientAllergy : BaseEntity
{
    public Guid PatientId { get; set; }
    public Patient Patient { get; set; } = null!;

    /// <summary>Substance, e.g. "Penicillin", "Latex".</summary>
    public string Substance { get; set; } = string.Empty;
    public AllergySeverity Severity { get; set; }
    public string? Notes { get; set; }
}
