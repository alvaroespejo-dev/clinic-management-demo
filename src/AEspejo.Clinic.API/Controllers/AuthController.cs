using AEspejo.Clinic.Application.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AEspejo.Clinic.API.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController(IAuthService auth) : ControllerBase
{
    /// <summary>Authenticates against the active tenant (requires subdomain or X-Tenant header) and returns a JWT.</summary>
    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<IActionResult> Login([FromBody] LoginRequest request, CancellationToken ct)
        => (await auth.LoginAsync(request, ct)).ToActionResult();
}
