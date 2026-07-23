using AEspejo.Clinic.Application.Common;
using AEspejo.Clinic.Application.Dtos;
using AEspejo.Clinic.Application.Interfaces;
using AEspejo.Clinic.Application.Repositories;
using AEspejo.Clinic.Domain.Entities;
using FluentValidation;
using Mapster;
using Microsoft.EntityFrameworkCore;

namespace AEspejo.Clinic.Application.Services;

public interface IServiceCatalogService
{
    Task<PagedResult<ServiceCatalogDto>> ListAsync(PagedRequest request, string lang, CancellationToken ct = default);
    Task<Result<ServiceCatalogDto>> GetByIdAsync(Guid id, string lang, CancellationToken ct = default);
    Task<Result<ServiceCatalogDto>> CreateAsync(CreateServiceCatalogDto dto, string lang, CancellationToken ct = default);
    Task<Result<ServiceCatalogDto>> UpdateAsync(Guid id, UpdateServiceCatalogDto dto, string lang, CancellationToken ct = default);
    Task<Result> DeleteAsync(Guid id, CancellationToken ct = default);
}

public class ServiceCatalogService(IAppDbContext db, IRepository<ServiceCatalog> repository, ITenantProvider tenant,
    IValidator<CreateServiceCatalogDto>? cv = null, IValidator<UpdateServiceCatalogDto>? uv = null)
    : IServiceCatalogService
{
    private readonly IAppDbContext _db = db;
    private readonly IRepository<ServiceCatalog> _repository = repository;
    private readonly ITenantProvider _tenant = tenant;
    private readonly IValidator<CreateServiceCatalogDto>? _createValidator = cv;
    private readonly IValidator<UpdateServiceCatalogDto>? _updateValidator = uv;

    public async Task<PagedResult<ServiceCatalogDto>> ListAsync(PagedRequest request, string lang, CancellationToken ct = default)
    {
        var query = _repository.Query().Include(s => s.Translations).AsQueryable();
        if (!request.IncludeInactive) query = query.Where(s => s.IsActive);
        if (!string.IsNullOrWhiteSpace(request.Search))
        {
            var s = request.Search.Trim();
            query = query.Where(x => x.Code.Contains(s) || x.Translations.Any(t => t.Name.Contains(s)));
        }

        var total = await query.CountAsync(ct);
        var items = await query
            .OrderByDescending(s => s.CreatedAt)
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToListAsync(ct);

        return new PagedResult<ServiceCatalogDto>
        {
            Items = items.Select(s => ToDto(s, lang)).ToList(),
            TotalCount = total,
            Page = request.Page,
            PageSize = request.PageSize
        };
    }

    public async Task<Result<ServiceCatalogDto>> GetByIdAsync(Guid id, string lang, CancellationToken ct = default)
    {
        var entity = await _repository.Query().Include(s => s.Translations).FirstOrDefaultAsync(s => s.Id == id, ct);
        return entity is null ? Result<ServiceCatalogDto>.NotFound() : Result<ServiceCatalogDto>.Ok(ToDto(entity, lang));
    }

    public async Task<Result<ServiceCatalogDto>> CreateAsync(CreateServiceCatalogDto dto, string lang, CancellationToken ct = default)
    {
        if (_createValidator is not null)
        {
            var v = await _createValidator.ValidateAsync(dto, ct);
            if (!v.IsValid) return Result<ServiceCatalogDto>.Invalid(v.Errors.Select(e => e.ErrorMessage).ToArray());
        }
        if (await _repository.Query().AnyAsync(s => s.Code == dto.Code, ct))
            return Result<ServiceCatalogDto>.Conflict("Ya existe un servicio con ese código.");

        var entity = new ServiceCatalog
        {
            Code = dto.Code,
            DefaultPrice = dto.DefaultPrice,
            Translations = dto.Translations.Select(t => new ServiceCatalogTranslation
            {
                LanguageCode = t.LanguageCode,
                Name = t.Name,
                Description = t.Description,
                Category = t.Category
            }).ToList()
        };
        await _repository.AddAsync(entity, ct);
        return Result<ServiceCatalogDto>.Ok(ToDto(entity, lang));
    }

    public async Task<Result<ServiceCatalogDto>> UpdateAsync(Guid id, UpdateServiceCatalogDto dto, string lang, CancellationToken ct = default)
    {
        if (_updateValidator is not null)
        {
            var v = await _updateValidator.ValidateAsync(dto, ct);
            if (!v.IsValid) return Result<ServiceCatalogDto>.Invalid(v.Errors.Select(e => e.ErrorMessage).ToArray());
        }

        var entity = await _repository.Query().Include(s => s.Translations).FirstOrDefaultAsync(s => s.Id == id, ct);
        if (entity is null) return Result<ServiceCatalogDto>.NotFound();

        entity.Code = dto.Code;
        entity.DefaultPrice = dto.DefaultPrice;
        entity.IsActive = dto.IsActive;

        // Replace the translation set.
        _db.ServiceCatalogTranslations.RemoveRange(entity.Translations);
        entity.Translations = dto.Translations.Select(t => new ServiceCatalogTranslation
        {
            ServiceId = entity.Id,
            LanguageCode = t.LanguageCode,
            Name = t.Name,
            Description = t.Description,
            Category = t.Category
        }).ToList();

        await _repository.UpdateAsync(entity, ct);
        return Result<ServiceCatalogDto>.Ok(ToDto(entity, lang));
    }

    public async Task<Result> DeleteAsync(Guid id, CancellationToken ct = default)
    {
        var entity = await _repository.GetByIdAsync(id, ct);
        if (entity is null) return Result.NotFound();
        entity.IsActive = false; // soft-delete
        await _repository.UpdateAsync(entity, ct);
        return Result.Ok();
    }

    /// <summary>Maps the entity resolving its texts to the requested language (falls back to the tenant's, then to the first translation).</summary>
    private ServiceCatalogDto ToDto(ServiceCatalog s, string lang)
    {
        var fallback = _tenant.DefaultLanguage ?? "es";
        var tr = s.Translations.FirstOrDefault(t => t.LanguageCode == lang)
                 ?? s.Translations.FirstOrDefault(t => t.LanguageCode == fallback)
                 ?? s.Translations.FirstOrDefault();

        return new ServiceCatalogDto
        {
            Id = s.Id,
            Code = s.Code,
            DefaultPrice = s.DefaultPrice,
            IsActive = s.IsActive,
            Name = tr?.Name ?? string.Empty,
            Description = tr?.Description,
            Category = tr?.Category ?? string.Empty,
            Translations = s.Translations.Select(t => t.Adapt<ServiceTranslationDto>()).ToList()
        };
    }
}
