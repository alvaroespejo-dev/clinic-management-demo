using AEspejo.Clinic.Application.Interfaces;
using AEspejo.Clinic.Application.Repositories;
using AEspejo.Clinic.Domain.Common;
using Microsoft.EntityFrameworkCore;

namespace AEspejo.Clinic.Infrastructure.Persistence;

/// <summary>
/// Generic repository implementation on top of IAppDbContext.
/// Write operations trigger SaveChangesAsync (and with it the AuditInterceptor).
/// For soft-delete entities use <see cref="SoftDeleteRepository{TEntity}"/>.
/// </summary>
public class Repository<TEntity>(IAppDbContext context) : IRepository<TEntity>
    where TEntity : BaseEntity
{
    protected IAppDbContext Context => context;

    /// <summary>Base query without filters; subclasses can override it.</summary>
    public virtual IQueryable<TEntity> Query() => context.Set<TEntity>().AsQueryable();

    public virtual async Task<TEntity?> GetByIdAsync(Guid id, CancellationToken ct = default)
        => await Query().FirstOrDefaultAsync(e => e.Id == id, ct);

    public virtual async Task<List<TEntity>> ListAsync(CancellationToken ct = default)
        => await Query().ToListAsync(ct);

    public virtual async Task<bool> ExistsAsync(Guid id, CancellationToken ct = default)
        => await Query().AnyAsync(e => e.Id == id, ct);

    public virtual async Task<int> CountAsync(CancellationToken ct = default)
        => await Query().CountAsync(ct);

    public virtual async Task<TEntity> AddAsync(TEntity entity, CancellationToken ct = default)
    {
        context.Set<TEntity>().Add(entity);
        await context.SaveChangesAsync(ct);
        return entity;
    }

    public virtual async Task UpdateAsync(TEntity entity, CancellationToken ct = default)
    {
        context.Set<TEntity>().Update(entity);
        await context.SaveChangesAsync(ct);
    }

    public virtual async Task DeleteAsync(TEntity entity, CancellationToken ct = default)
    {
        context.Set<TEntity>().Remove(entity);
        await context.SaveChangesAsync(ct);
    }

    public virtual async Task DeleteByIdAsync(Guid id, CancellationToken ct = default)
    {
        var entity = await GetByIdAsync(id, ct);
        if (entity is not null)
            await DeleteAsync(entity, ct);
    }
}
