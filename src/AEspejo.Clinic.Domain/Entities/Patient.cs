using AEspejo.Clinic.Domain.Common;
using AEspejo.Clinic.Domain.Enums;

namespace AEspejo.Clinic.Domain.Entities;

/// <summary>Clinic patient (may not have a login).</summary>
public class Patient : BaseEntity, ISoftDeletable
{
    public Guid BranchId { get; set; }
    public Branch Branch { get; set; } = null!;

    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public DateOnly DateOfBirth { get; set; }
    public Gender Gender { get; set; }

    public DocumentType DocumentType { get; set; }
    public string DocumentNumber { get; set; } = string.Empty;

    public string Phone { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;

    public string? BloodType { get; set; }

    /// <summary>Language for communications/reminders (ISO 639-1), e.g. "es".</summary>
    public string PreferredLanguage { get; set; } = "es";

    public string? EmergencyContactName { get; set; }
    public string? EmergencyContactPhone { get; set; }
    public string? Notes { get; set; }

    public bool IsActive { get; set; } = true;

    public ICollection<PatientAllergy> Allergies { get; set; } = new List<PatientAllergy>();
    public ICollection<MedicalCondition> MedicalConditions { get; set; } = new List<MedicalCondition>();
    public Odontogram? Odontogram { get; set; }
    public ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();
    public ICollection<TreatmentPlan> TreatmentPlans { get; set; } = new List<TreatmentPlan>();
    public ICollection<Invoice> Invoices { get; set; } = new List<Invoice>();
}
