using AEspejo.Clinic.Domain.Enums;

namespace AEspejo.Clinic.Application.Dtos;

// ---------- ServiceCatalog + translations ----------
public class ServiceTranslationDto
{
    public string LanguageCode { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string Category { get; set; } = string.Empty;
}

public class CreateServiceCatalogDto
{
    public string Code { get; set; } = string.Empty;
    public decimal DefaultPrice { get; set; }
    public List<ServiceTranslationDto> Translations { get; set; } = new();
}

public class UpdateServiceCatalogDto
{
    public string Code { get; set; } = string.Empty;
    public decimal DefaultPrice { get; set; }
    public bool IsActive { get; set; } = true;
    public List<ServiceTranslationDto> Translations { get; set; } = new();
}

public class ServiceCatalogDto
{
    public Guid Id { get; set; }
    public string Code { get; set; } = string.Empty;
    public decimal DefaultPrice { get; set; }
    public bool IsActive { get; set; }
    /// <summary>Name resolved in the requested language (falls back to the default language).</summary>
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string Category { get; set; } = string.Empty;
    public List<ServiceTranslationDto> Translations { get; set; } = new();
}

// ---------- Appointment ----------
public class CreateAppointmentDto
{
    public Guid BranchId { get; set; }
    public Guid PatientId { get; set; }
    public Guid ProfessionalId { get; set; }
    public Guid? RoomId { get; set; }
    public DateTimeOffset ScheduledAt { get; set; }
    public int DurationMinutes { get; set; }
    public string? Notes { get; set; }
}

public class UpdateAppointmentDto
{
    public Guid? RoomId { get; set; }
    public DateTimeOffset ScheduledAt { get; set; }
    public int DurationMinutes { get; set; }
    public AppointmentStatus Status { get; set; }
    public string? Notes { get; set; }
    public string? CancelReason { get; set; }
}

public class AppointmentDto
{
    public Guid Id { get; set; }
    public Guid BranchId { get; set; }
    public Guid PatientId { get; set; }
    public Guid ProfessionalId { get; set; }
    public Guid? RoomId { get; set; }
    public DateTimeOffset ScheduledAt { get; set; }
    public int DurationMinutes { get; set; }
    public AppointmentStatus Status { get; set; }
    public string? Notes { get; set; }
    public string? CancelReason { get; set; }
    public Guid CreatedByUserId { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
}

// ---------- AppointmentDetail ----------
public class CreateAppointmentDetailDto
{
    public Guid AppointmentId { get; set; }
    public Guid ServiceId { get; set; }
    public int? ToothNumber { get; set; }
    public int Quantity { get; set; } = 1;
    public decimal UnitPrice { get; set; }
    public string? Notes { get; set; }
}

public class UpdateAppointmentDetailDto
{
    public int? ToothNumber { get; set; }
    public int Quantity { get; set; } = 1;
    public decimal UnitPrice { get; set; }
    public string? Notes { get; set; }
}

public class AppointmentDetailDto
{
    public Guid Id { get; set; }
    public Guid AppointmentId { get; set; }
    public Guid ServiceId { get; set; }
    public int? ToothNumber { get; set; }
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public string? Notes { get; set; }
}

// ---------- TreatmentPlan ----------
public class CreateTreatmentPlanDto
{
    public Guid PatientId { get; set; }
    public Guid ProfessionalId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
}

public class UpdateTreatmentPlanDto
{
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public TreatmentPlanStatus Status { get; set; }
}

public class TreatmentPlanDto
{
    public Guid Id { get; set; }
    public Guid PatientId { get; set; }
    public Guid ProfessionalId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public TreatmentPlanStatus Status { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
}

// ---------- TreatmentPlanItem ----------
public class CreateTreatmentPlanItemDto
{
    public Guid TreatmentPlanId { get; set; }
    public Guid ServiceId { get; set; }
    public int? ToothNumber { get; set; }
    public int Quantity { get; set; } = 1;
    public decimal EstimatedPrice { get; set; }
}

public class UpdateTreatmentPlanItemDto
{
    public int? ToothNumber { get; set; }
    public int Quantity { get; set; } = 1;
    public decimal EstimatedPrice { get; set; }
    public TreatmentPlanItemStatus Status { get; set; }
    public Guid? AppointmentDetailId { get; set; }
}

public class TreatmentPlanItemDto
{
    public Guid Id { get; set; }
    public Guid TreatmentPlanId { get; set; }
    public Guid ServiceId { get; set; }
    public int? ToothNumber { get; set; }
    public int Quantity { get; set; }
    public decimal EstimatedPrice { get; set; }
    public TreatmentPlanItemStatus Status { get; set; }
    public Guid? AppointmentDetailId { get; set; }
}
