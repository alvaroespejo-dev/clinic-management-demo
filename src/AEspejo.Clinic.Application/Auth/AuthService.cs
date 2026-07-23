using System.Security.Claims;
using System.Text;
using AEspejo.Clinic.Application.Common;
using AEspejo.Clinic.Application.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;

namespace AEspejo.Clinic.Application.Auth;

public class AuthService(IAppDbContext db, ITenantProvider tenant, IOptions<JwtSettings> jwt) : IAuthService
{
    private readonly JwtSettings _jwt = jwt.Value;

    public async Task<Result<LoginResponse>> LoginAsync(LoginRequest request, CancellationToken ct = default)
    {
        var user = await db.Users
            .FirstOrDefaultAsync(u => u.Email == request.Email && u.IsActive, ct);

        if (user is null || !PasswordHasher.Verify(request.Password, user.PasswordHash))
            return Result<LoginResponse>.Unauthorized("Credenciales inválidas.");

        var expiresAt = DateTimeOffset.UtcNow.AddMinutes(_jwt.ExpiryMinutes);
        var token = GenerateToken(user.Id, user.Email, user.Role.ToString(), user.BranchId, expiresAt);

        return Result<LoginResponse>.Ok(new LoginResponse
        {
            Token = token,
            ExpiresAt = expiresAt,
            UserId = user.Id,
            FullName = $"{user.FirstName} {user.LastName}".Trim(),
            Role = user.Role,
            PreferredLanguage = user.PreferredLanguage
        });
    }

    private string GenerateToken(Guid userId, string email, string role, Guid? branchId, DateTimeOffset expiresAt)
    {
        List<Claim> claims =
        [
            new(JwtRegisteredClaimNames.Sub, userId.ToString()),
            new(JwtRegisteredClaimNames.Email, email),
            new(ClaimTypes.Role, role),
            new("tenant", tenant.TenantIdentifier ?? string.Empty)
        ];
        if (branchId is not null)
            claims.Add(new Claim("branchId", branchId.Value.ToString()));

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwt.SecretKey));

        var descriptor = new SecurityTokenDescriptor
        {
            Issuer = _jwt.Issuer,
            Audience = _jwt.Audience,
            Subject = new ClaimsIdentity(claims),
            Expires = expiresAt.UtcDateTime,
            SigningCredentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256)
        };

        return new JsonWebTokenHandler().CreateToken(descriptor);
    }
}
