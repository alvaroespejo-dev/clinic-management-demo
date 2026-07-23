using AEspejo.Clinic.Application.Dtos;
using AEspejo.Clinic.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AEspejo.Clinic.API.Controllers;

/// <summary>
/// REST API endpoints for managing organization configuration.
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ConfigController(ConfigService configService) : ControllerBase
{
    /// <summary>
    /// Retrieve the current organization configuration.
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<OrgConfigDto>> GetConfig(CancellationToken ct)
    {
        var config = await configService.GetConfigAsync(ct);
        return Ok(config);
    }

    /// <summary>
    /// Update the organization configuration with clinic name and logo.
    /// </summary>
    [HttpPut]
    public async Task<ActionResult<OrgConfigDto>> UpdateConfig(UpdateOrgConfigDto dto, CancellationToken ct)
    {
        var config = await configService.UpdateConfigAsync(dto, ct);
        return Ok(config);
    }
}
