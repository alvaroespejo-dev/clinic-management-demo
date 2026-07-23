namespace AEspejo.Clinic.Application.Interfaces;

/// <summary>
/// Exposes the identity of the user behind the current request.
/// The concrete implementation lives in the API layer (reads the JWT/HttpContext).
/// </summary>
public interface ICurrentUserService
{
    Guid? UserId { get; }
    string? IpAddress { get; }
}
