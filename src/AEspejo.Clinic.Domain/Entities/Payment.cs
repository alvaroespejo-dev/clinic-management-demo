using AEspejo.Clinic.Domain.Common;
using AEspejo.Clinic.Domain.Enums;

namespace AEspejo.Clinic.Domain.Entities;

/// <summary>Payment (full or partial) applied to an invoice.</summary>
public class Payment : BaseEntity
{
    public Guid InvoiceId { get; set; }
    public Invoice Invoice { get; set; } = null!;

    public decimal Amount { get; set; }
    public PaymentMethod Method { get; set; }
    public DateTimeOffset PaidAt { get; set; }
    /// <summary>Transfer/voucher number, if applicable.</summary>
    public string? Reference { get; set; }

    public Guid ReceivedByUserId { get; set; }
    public User ReceivedByUser { get; set; } = null!;
}
