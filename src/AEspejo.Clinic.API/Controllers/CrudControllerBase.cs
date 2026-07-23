using AEspejo.Clinic.Application.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AEspejo.Clinic.API.Controllers;

/// <summary>
/// Generic CRUD controller. Each entity exposes a subclass with its own route.
/// Requires authentication; subclasses can tighten it with [Authorize(Roles=...)].
/// </summary>
[ApiController]
[Authorize]
public abstract class CrudControllerBase<TCreateDto, TUpdateDto, TResponseDto>(
    ICrudService<TCreateDto, TUpdateDto, TResponseDto> service) : ControllerBase
{
    protected ICrudService<TCreateDto, TUpdateDto, TResponseDto> Service => service;

    [HttpGet]
    public async Task<IActionResult> List([FromQuery] PagedRequest request, CancellationToken ct)
        => Ok(await Service.ListAsync(request, ct));

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> Get(Guid id, CancellationToken ct)
        => (await Service.GetByIdAsync(id, ct)).ToActionResult();

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] TCreateDto dto, CancellationToken ct)
        => (await Service.CreateAsync(dto, ct)).ToCreatedResult();

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] TUpdateDto dto, CancellationToken ct)
        => (await Service.UpdateAsync(id, dto, ct)).ToActionResult();

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
        => (await Service.DeleteAsync(id, ct)).ToActionResult();
}
