using AEspejo.Clinic.Application.Common;
using AEspejo.Clinic.Application.Dtos;
using AEspejo.Clinic.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AEspejo.Clinic.API.Controllers;

[ApiController]
[Authorize]
[Route("api/services")]
public class ServiceCatalogController(IServiceCatalogService service) : ControllerBase
{
    /// <summary>Requested language: ?lang= query string or Accept-Language; falls back to "es".</summary>
    private string Lang()
    {
        if (Request.Query.TryGetValue("lang", out var q) && !string.IsNullOrWhiteSpace(q))
            return q.ToString().Trim().ToLowerInvariant();

        var accept = Request.Headers.AcceptLanguage.ToString();
        if (!string.IsNullOrWhiteSpace(accept))
            return accept.Split(',')[0].Split('-')[0].Trim().ToLowerInvariant();

        return "es";
    }

    [HttpGet]
    public async Task<IActionResult> List([FromQuery] PagedRequest request, CancellationToken ct)
        => Ok(await service.ListAsync(request, Lang(), ct));

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> Get(Guid id, CancellationToken ct)
        => (await service.GetByIdAsync(id, Lang(), ct)).ToActionResult();

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Create([FromBody] CreateServiceCatalogDto dto, CancellationToken ct)
        => (await service.CreateAsync(dto, Lang(), ct)).ToCreatedResult();

    [HttpPut("{id:guid}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateServiceCatalogDto dto, CancellationToken ct)
        => (await service.UpdateAsync(id, dto, Lang(), ct)).ToActionResult();

    [HttpDelete("{id:guid}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
        => (await service.DeleteAsync(id, ct)).ToActionResult();
}
