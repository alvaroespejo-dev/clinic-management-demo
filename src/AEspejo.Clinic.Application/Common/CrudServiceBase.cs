using AEspejo.Clinic.Application.Interfaces;
using AEspejo.Clinic.Application.Repositories;
using AEspejo.Clinic.Domain.Common;
using FluentValidation;
using Mapster;
using Microsoft.EntityFrameworkCore;

namespace AEspejo.Clinic.Application.Common;

/// <summary>
/// Generic CRUD for manageable entities. Maps with Mapster and validates with FluentValidation.
/// Subclasses can override <see cref="ApplySearch"/> and <see cref="BaseQuery"/>.
/// Deletion is physical; for soft-delete use <see cref="SoftDeleteCrudServiceBase{TEntity,TCreateDto,TUpdateDto,TResponseDto}"/>.
/// Accesses data through IRepository{TEntity}; Db stays exposed for cross-entity queries.
/// </summary>
public abstract class CrudServiceBase<TEntity, TCreateDto, TUpdateDto, TResponseDto>(
    IAppDbContext db,
    IRepository<TEntity> repository,
    IValidator<TCreateDto>? createValidator = null,
    IValidator<TUpdateDto>? updateValidator = null)
    : ICrudService<TCreateDto, TUpdateDto, TResponseDto>
    where TEntity : BaseEntity
{
    protected IAppDbContext Db => db;
    protected IRepository<TEntity> Repository => repository;

    /// <summary>Base query (subclasses can add Includes). Uses Repository by default.</summary>
    protected virtual IQueryable<TEntity> BaseQuery() => repository.Query();

    /// <summary>Free-text search filter. Does not filter by default; subclasses override it.</summary>
    protected virtual IQueryable<TEntity> ApplySearch(IQueryable<TEntity> query, string search) => query;

    /// <summary>Extension point to exclude inactive records, etc. Does not filter by default.</summary>
    protected virtual IQueryable<TEntity> ApplyListFilter(IQueryable<TEntity> query, PagedRequest request) => query;

    public virtual async Task<PagedResult<TResponseDto>> ListAsync(PagedRequest request, CancellationToken ct = default)
    {
        var query = ApplyListFilter(BaseQuery(), request);

        if (!string.IsNullOrWhiteSpace(request.Search))
            query = ApplySearch(query, request.Search.Trim());

        var total = await query.CountAsync(ct);
        var items = await query
            .OrderByDescending(e => e.CreatedAt)
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToListAsync(ct);

        return new PagedResult<TResponseDto>
        {
            Items = items.Adapt<List<TResponseDto>>(),
            TotalCount = total,
            Page = request.Page,
            PageSize = request.PageSize
        };
    }

    public virtual async Task<Result<TResponseDto>> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        var entity = await BaseQuery().FirstOrDefaultAsync(e => e.Id == id, ct);
        return entity is null
            ? Result<TResponseDto>.NotFound()
            : Result<TResponseDto>.Ok(entity.Adapt<TResponseDto>());
    }

    public virtual async Task<Result<TResponseDto>> CreateAsync(TCreateDto dto, CancellationToken ct = default)
    {
        if (createValidator is not null)
        {
            var validation = await createValidator.ValidateAsync(dto, ct);
            if (!validation.IsValid)
                return Result<TResponseDto>.Invalid([.. validation.Errors.Select(e => e.ErrorMessage)]);
        }

        var entity = dto.Adapt<TEntity>()!;
        await repository.AddAsync(entity, ct);
        return Result<TResponseDto>.Ok(entity.Adapt<TResponseDto>());
    }

    public virtual async Task<Result<TResponseDto>> UpdateAsync(Guid id, TUpdateDto dto, CancellationToken ct = default)
    {
        if (updateValidator is not null)
        {
            var validation = await updateValidator.ValidateAsync(dto, ct);
            if (!validation.IsValid)
                return Result<TResponseDto>.Invalid([.. validation.Errors.Select(e => e.ErrorMessage)]);
        }

        var entity = await repository.GetByIdAsync(id, ct);
        if (entity is null) return Result<TResponseDto>.NotFound();

        dto.Adapt(entity);
        await repository.UpdateAsync(entity, ct);
        return Result<TResponseDto>.Ok(entity.Adapt<TResponseDto>());
    }

    public virtual async Task<Result> DeleteAsync(Guid id, CancellationToken ct = default)
    {
        var entity = await repository.GetByIdAsync(id, ct);
        if (entity is null) return Result.NotFound();

        await repository.DeleteAsync(entity, ct);
        return Result.Ok();
    }
}

/// <summary>
/// Generic CRUD with soft-delete: List excludes inactive records unless the request
/// asks for IncludeInactive. The logical delete (IsActive = false) is applied by the
/// SoftDeleteRepository{TEntity} registered in DI for the entity.
/// </summary>
public abstract class SoftDeleteCrudServiceBase<TEntity, TCreateDto, TUpdateDto, TResponseDto>(
    IAppDbContext db,
    IRepository<TEntity> repository,
    IValidator<TCreateDto>? createValidator = null,
    IValidator<TUpdateDto>? updateValidator = null)
    : CrudServiceBase<TEntity, TCreateDto, TUpdateDto, TResponseDto>(db, repository, createValidator, updateValidator)
    where TEntity : BaseEntity, ISoftDeletable
{
    protected override IQueryable<TEntity> ApplyListFilter(IQueryable<TEntity> query, PagedRequest request)
        => request.IncludeInactive ? query : query.Where(e => e.IsActive);
}
