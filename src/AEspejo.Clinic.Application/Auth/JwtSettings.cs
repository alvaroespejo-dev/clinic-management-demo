namespace AEspejo.Clinic.Application.Auth;

/// <summary>JWT issuing/validation settings (from appsettings: "Jwt" section).</summary>
public class JwtSettings
{
    public string Issuer { get; set; } = string.Empty;
    public string Audience { get; set; } = string.Empty;
    public string SecretKey { get; set; } = string.Empty;
    public int ExpiryMinutes { get; set; } = 480;
}
