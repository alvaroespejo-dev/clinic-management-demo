using AEspejo.Clinic.Domain.Enums;

namespace AEspejo.Clinic.Application.Dtos;

// ---------- Invoice ----------
public class CreateInvoiceDto
{
    public Guid BranchId { get; set; }
    public Guid PatientId { get; set; }
    public Guid? AppointmentId { get; set; }
    public string InvoiceNumber { get; set; } = string.Empty;
    public DateOnly IssueDate { get; set; }
    public decimal TotalAmount { get; set; }
    public string? Notes { get; set; }
}

public class UpdateInvoiceDto
{
    public string InvoiceNumber { get; set; } = string.Empty;
    public DateOnly IssueDate { get; set; }
    public decimal TotalAmount { get; set; }
    public InvoiceStatus Status { get; set; }
    public string? Notes { get; set; }
}

public class InvoiceDto
{
    public Guid Id { get; set; }
    public Guid BranchId { get; set; }
    public Guid PatientId { get; set; }
    public Guid? AppointmentId { get; set; }
    public string InvoiceNumber { get; set; } = string.Empty;
    public DateOnly IssueDate { get; set; }
    public decimal TotalAmount { get; set; }
    public decimal PaidAmount { get; set; }
    public InvoiceStatus Status { get; set; }
    public string? Notes { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
}

// ---------- Payment ----------
public class CreatePaymentDto
{
    public Guid InvoiceId { get; set; }
    public decimal Amount { get; set; }
    public PaymentMethod Method { get; set; }
    public DateTimeOffset PaidAt { get; set; }
    public string? Reference { get; set; }
}

public class UpdatePaymentDto
{
    public decimal Amount { get; set; }
    public PaymentMethod Method { get; set; }
    public DateTimeOffset PaidAt { get; set; }
    public string? Reference { get; set; }
}

public class PaymentDto
{
    public Guid Id { get; set; }
    public Guid InvoiceId { get; set; }
    public decimal Amount { get; set; }
    public PaymentMethod Method { get; set; }
    public DateTimeOffset PaidAt { get; set; }
    public string? Reference { get; set; }
    public Guid ReceivedByUserId { get; set; }
}

// ---------- AuditLog (read-only) ----------
public class AuditLogDto
{
    public long Id { get; set; }
    public string TableName { get; set; } = string.Empty;
    public string RecordId { get; set; } = string.Empty;
    public AuditAction Action { get; set; }
    public string? OldValues { get; set; }
    public string? NewValues { get; set; }
    public Guid? UserId { get; set; }
    public DateTimeOffset ChangedAt { get; set; }
    public string? IpAddress { get; set; }
}
