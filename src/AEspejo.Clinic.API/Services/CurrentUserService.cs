using System.Security.Claims;
using AEspejo.Clinic.Application.Interfaces;

namespace AEspejo.Clinic.API.Services;

/// <summary>Reads the user identity from the JWT (HttpContext) for the audit interceptor.</summary>
public class CurrentUserService : ICurrentUserService
{
    private readonly IHttpContextAccessor _accessor;

    public CurrentUserService(IHttpContextAccessor accessor) => _accessor = accessor;

    public Guid? UserId
    {
        get
        {
            var sub = _accessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier)
                      ?? _accessor.HttpContext?.User.FindFirstValue("sub");
            return Guid.TryParse(sub, out var id) ? id : null;
        }
    }

    public string? IpAddress => _accessor.HttpContext?.Connection.RemoteIpAddress?.ToString();
}
