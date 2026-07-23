using AEspejo.Clinic.Domain.Enums;

namespace AEspejo.Clinic.Application.Dtos;

// ---------- Patient ----------
public class CreatePatientDto
{
    public Guid BranchId { get; set; }
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
    public string PreferredLanguage { get; set; } = "es";
    public string? EmergencyContactName { get; set; }
    public string? EmergencyContactPhone { get; set; }
    public string? Notes { get; set; }
}

public class UpdatePatientDto
{
    public Guid BranchId { get; set; }
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
    public string PreferredLanguage { get; set; } = "es";
    public string? EmergencyContactName { get; set; }
    public string? EmergencyContactPhone { get; set; }
    public string? Notes { get; set; }
    public bool IsActive { get; set; } = true;
}

public class PatientDto
{
    public Guid Id { get; set; }
    public Guid BranchId { get; set; }
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
    public string PreferredLanguage { get; set; } = "es";
    public string? EmergencyContactName { get; set; }
    public string? EmergencyContactPhone { get; set; }
    public string? Notes { get; set; }
    public bool IsActive { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
}

// ---------- PatientAllergy ----------
public class CreatePatientAllergyDto
{
    public Guid PatientId { get; set; }
    public string Substance { get; set; } = string.Empty;
    public AllergySeverity Severity { get; set; }
    public string? Notes { get; set; }
}

public class UpdatePatientAllergyDto
{
    public string Substance { get; set; } = string.Empty;
    public AllergySeverity Severity { get; set; }
    public string? Notes { get; set; }
}

public class PatientAllergyDto
{
    public Guid Id { get; set; }
    public Guid PatientId { get; set; }
    public string Substance { get; set; } = string.Empty;
    public AllergySeverity Severity { get; set; }
    public string? Notes { get; set; }
}

// ---------- MedicalCondition ----------
public class CreateMedicalConditionDto
{
    public Guid PatientId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Notes { get; set; }
}

public class UpdateMedicalConditionDto
{
    public string Name { get; set; } = string.Empty;
    public string? Notes { get; set; }
    public bool IsActive { get; set; } = true;
}

public class MedicalConditionDto
{
    public Guid Id { get; set; }
    public Guid PatientId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Notes { get; set; }
    public bool IsActive { get; set; }
}

// ---------- Odontogram ----------
public class CreateOdontogramDto
{
    public Guid PatientId { get; set; }
}

/// <summary>The odontogram has no directly editable fields; it is managed through its ToothRecords.</summary>
public class UpdateOdontogramDto { }

public class OdontogramDto
{
    public Guid Id { get; set; }
    public Guid PatientId { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
}

// ---------- ToothRecord ----------
public class CreateToothRecordDto
{
    public Guid OdontogramId { get; set; }
    public int ToothNumber { get; set; }
    public ToothSurface Surface { get; set; }
    public ToothStatus Status { get; set; }
    public string? Notes { get; set; }
}

public class UpdateToothRecordDto
{
    public int ToothNumber { get; set; }
    public ToothSurface Surface { get; set; }
    public ToothStatus Status { get; set; }
    public string? Notes { get; set; }
}

public class ToothRecordDto
{
    public Guid Id { get; set; }
    public Guid OdontogramId { get; set; }
    public int ToothNumber { get; set; }
    public ToothSurface Surface { get; set; }
    public ToothStatus Status { get; set; }
    public string? Notes { get; set; }
    public Guid UpdatedByUserId { get; set; }
    public DateTimeOffset? UpdatedAt { get; set; }
}
