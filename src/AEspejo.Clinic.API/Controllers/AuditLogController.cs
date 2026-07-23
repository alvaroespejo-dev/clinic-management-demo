using AEspejo.Clinic.Application.Common;
using AEspejo.Clinic.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AEspejo.Clinic.API.Controllers;

/// <summary>Audit log: read-only, restricted to administrators.</summary>
[ApiController]
[Authorize(Roles = "Admin")]
[Route("api/audit-logs")]
public class AuditLogController(IAuditLogService service) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> List([FromQuery] PagedRequest request, CancellationToken ct)
        => Ok(await service.ListAsync(request, ct));

    [HttpGet("{id:long}")]
    public async Task<IActionResult> Get(long id, CancellationToken ct)
        => (await service.GetByIdAsync(id, ct)).ToActionResult();
}
