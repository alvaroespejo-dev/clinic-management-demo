using AEspejo.Clinic.Domain.Enums;

namespace AEspejo.Clinic.Application.Dtos;

// ---------- Branch ----------
public class CreateBranchDto
{
    public string Name { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
}

public class UpdateBranchDto
{
    public string Name { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;
}

public class BranchDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public bool IsActive { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset? UpdatedAt { get; set; }
}

// ---------- Room ----------
public class CreateRoomDto
{
    public Guid BranchId { get; set; }
    public string Name { get; set; } = string.Empty;
}

public class UpdateRoomDto
{
    public Guid BranchId { get; set; }
    public string Name { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;
}

public class RoomDto
{
    public Guid Id { get; set; }
    public Guid BranchId { get; set; }
    /// <summary>Branch name (flattened from Branch.Name by Mapster).</summary>
    public string BranchName { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public bool IsActive { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
}

// ---------- User ----------
public class CreateUserDto
{
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public UserRole Role { get; set; }
    public string PreferredLanguage { get; set; } = "es";
    public Guid? BranchId { get; set; }
}

public class UpdateUserDto
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public UserRole Role { get; set; }
    public string PreferredLanguage { get; set; } = "es";
    public Guid? BranchId { get; set; }
    public bool IsActive { get; set; } = true;
}

public class UserDto
{
    public Guid Id { get; set; }
    public string Email { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public UserRole Role { get; set; }
    public string PreferredLanguage { get; set; } = "es";
    public Guid? BranchId { get; set; }
    public bool IsActive { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
}

// ---------- Professional (1:1 with User) ----------
public class CreateProfessionalDto
{
    /// <summary>The professional's UserId (1:1 relationship). The User must already exist.</summary>
    public Guid UserId { get; set; }
    public string LicenseNumber { get; set; } = string.Empty;
    public string Specialty { get; set; } = string.Empty;
    public string Color { get; set; } = "#3b82f6";
}

public class UpdateProfessionalDto
{
    public string LicenseNumber { get; set; } = string.Empty;
    public string Specialty { get; set; } = string.Empty;
    public string Color { get; set; } = "#3b82f6";
}

public class ProfessionalDto
{
    public Guid Id { get; set; }
    public string LicenseNumber { get; set; } = string.Empty;
    public string Specialty { get; set; } = string.Empty;
    public string Color { get; set; } = string.Empty;
    /// <summary>User's first name (flattened from User.FirstName).</summary>
    public string FirstName { get; set; } = string.Empty;
    /// <summary>User's last name (flattened from User.LastName).</summary>
    public string LastName { get; set; } = string.Empty;
}
