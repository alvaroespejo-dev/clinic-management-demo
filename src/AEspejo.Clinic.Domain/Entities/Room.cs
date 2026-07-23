using AEspejo.Clinic.Domain.Common;

namespace AEspejo.Clinic.Domain.Entities;

/// <summary>Treatment room / chair inside a branch.</summary>
public class Room : BaseEntity, ISoftDeletable
{
    public Guid BranchId { get; set; }
    public Branch Branch { get; set; } = null!;

    public string Name { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;

    public ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();
}
