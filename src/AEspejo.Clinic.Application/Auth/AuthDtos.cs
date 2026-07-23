using AEspejo.Clinic.Domain.Enums;

namespace AEspejo.Clinic.Application.Auth;

public class LoginRequest
{
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}

public class LoginResponse
{
    public string Token { get; set; } = string.Empty;
    public DateTimeOffset ExpiresAt { get; set; }
    public Guid UserId { get; set; }
    public string FullName { get; set; } = string.Empty;
    public UserRole Role { get; set; }
    public string PreferredLanguage { get; set; } = "es";
}
