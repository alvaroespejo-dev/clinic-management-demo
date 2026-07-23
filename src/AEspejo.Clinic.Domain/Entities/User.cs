using AEspejo.Clinic.Domain.Common;
using AEspejo.Clinic.Domain.Enums;

namespace AEspejo.Clinic.Domain.Entities;

/// <summary>User who signs in to the system (has a login).</summary>
public class User : BaseEntity, ISoftDeletable
{
    public string Email { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public UserRole Role { get; set; }

    /// <summary>UI language for this user (ISO 639-1), e.g. "es".</summary>
    public string PreferredLanguage { get; set; } = "es";

    /// <summary>Assigned branch. Nullable: a global admin is not tied to a branch.</summary>
    public Guid? BranchId { get; set; }
    public Branch? Branch { get; set; }

    public bool IsActive { get; set; } = true;

    /// <summary>Additional clinical data when the user is a professional (1:1).</summary>
    public Professional? Professional { get; set; }
}
