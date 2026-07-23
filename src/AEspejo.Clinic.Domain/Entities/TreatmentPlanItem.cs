using AEspejo.Clinic.Domain.Common;
using AEspejo.Clinic.Domain.Enums;

namespace AEspejo.Clinic.Domain.Entities;

/// <summary>Item (planned procedure) within a treatment plan.</summary>
public class TreatmentPlanItem : BaseEntity
{
    public Guid TreatmentPlanId { get; set; }
    public TreatmentPlan TreatmentPlan { get; set; } = null!;

    public Guid ServiceId { get; set; }
    public ServiceCatalog Service { get; set; } = null!;

    /// <summary>Target tooth (FDI notation), if applicable.</summary>
    public int? ToothNumber { get; set; }
    public int Quantity { get; set; } = 1;
    public decimal EstimatedPrice { get; set; }
    public TreatmentPlanItemStatus Status { get; set; } = TreatmentPlanItemStatus.Pending;

    /// <summary>Appointment detail that executed this item (plan → execution traceability).</summary>
    public Guid? AppointmentDetailId { get; set; }
    public AppointmentDetail? AppointmentDetail { get; set; }
}
