using AEspejo.Clinic.Domain.Common;

namespace AEspejo.Clinic.Domain.Entities;

/// <summary>Patient odontogram; groups the state of every tooth.</summary>
public class Odontogram : BaseEntity
{
    public Guid PatientId { get; set; }
    public Patient Patient { get; set; } = null!;

    public ICollection<ToothRecord> ToothRecords { get; set; } = new List<ToothRecord>();
}
