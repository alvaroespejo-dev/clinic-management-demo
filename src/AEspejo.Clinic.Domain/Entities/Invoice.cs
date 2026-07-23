using AEspejo.Clinic.Domain.Common;
using AEspejo.Clinic.Domain.Enums;

namespace AEspejo.Clinic.Domain.Entities;

/// <summary>Invoice issued to a patient.</summary>
public class Invoice : BaseEntity
{
    public Guid BranchId { get; set; }
    public Branch Branch { get; set; } = null!;

    public Guid PatientId { get; set; }
    public Patient Patient { get; set; } = null!;

    /// <summary>Related appointment (optional: the invoice can be manual).</summary>
    public Guid? AppointmentId { get; set; }
    public Appointment? Appointment { get; set; }

    public string InvoiceNumber { get; set; } = string.Empty;
    public DateOnly IssueDate { get; set; }
    public decimal TotalAmount { get; set; }
    public decimal PaidAmount { get; set; }
    public InvoiceStatus Status { get; set; } = InvoiceStatus.Draft;
    public string? Notes { get; set; }

    public ICollection<Payment> Payments { get; set; } = new List<Payment>();
}
