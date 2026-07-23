using AEspejo.Clinic.Application.Common;

namespace AEspejo.Clinic.Application.Auth;

public interface IAuthService
{
    Task<Result<LoginResponse>> LoginAsync(LoginRequest request, CancellationToken ct = default);
}
