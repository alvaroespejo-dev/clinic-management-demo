namespace AEspejo.Clinic.Domain.Enums;

public enum UserRole
{
    Admin = 1,
    Dentist = 2,
    Assistant = 3,
    Receptionist = 4
}

public enum Gender
{
    Male = 1,
    Female = 2,
    Other = 3
}

public enum DocumentType
{
    NationalId = 1,
    Passport = 2,
    Other = 99
}

public enum AllergySeverity
{
    Mild = 1,
    Moderate = 2,
    Severe = 3
}

public enum ToothSurface
{
    Buccal = 1,
    Lingual = 2,
    Mesial = 3,
    Distal = 4,
    Occlusal = 5,
    Full = 6
}

public enum ToothStatus
{
    Healthy = 1,
    Caries = 2,
    Filled = 3,
    Extracted = 4,
    Crown = 5,
    Implant = 6,
    RootCanal = 7,
    Missing = 8
}

public enum AppointmentStatus
{
    Scheduled = 1,
    Confirmed = 2,
    InProgress = 3,
    Completed = 4,
    Cancelled = 5,
    NoShow = 6
}

public enum TreatmentPlanStatus
{
    Draft = 1,
    Active = 2,
    Completed = 3,
    Cancelled = 4
}

public enum TreatmentPlanItemStatus
{
    Pending = 1,
    InProgress = 2,
    Completed = 3,
    Skipped = 4
}

public enum InvoiceStatus
{
    Draft = 1,
    Issued = 2,
    PartiallyPaid = 3,
    Paid = 4,
    Cancelled = 5
}

public enum PaymentMethod
{
    Cash = 1,
    Card = 2,
    Transfer = 3,
    Insurance = 4
}

public enum AuditAction
{
    Created = 1,
    Updated = 2,
    Deleted = 3
}
