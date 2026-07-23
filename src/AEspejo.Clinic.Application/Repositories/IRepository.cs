using AEspejo.Clinic.Domain.Common;

namespace AEspejo.Clinic.Application.Repositories;

/// <summary>
/// Generic repository interface for data access abstraction.
/// Query() returns IQueryable for advanced filtering; write operations trigger SaveChangesAsync.
/// Specialized for BaseEntity and domain model entities.
/// </summary>
public interface IRepository<TEntity> where TEntity : BaseEntity
{
    /// <summary>Returns queryable for building complex queries (no SaveChanges).</summary>
    IQueryable<TEntity> Query();

    /// <summary>Get entity by ID; returns null if not found.</summary>
    Task<TEntity?> GetByIdAsync(Guid id, CancellationToken ct = default);

    /// <summary>Get all entities.</summary>
    Task<List<TEntity>> ListAsync(CancellationToken ct = default);

    /// <summary>Check if entity exists by ID.</summary>
    Task<bool> ExistsAsync(Guid id, CancellationToken ct = default);

    /// <summary>Count total entities matching query.</summary>
    Task<int> CountAsync(CancellationToken ct = default);

    /// <summary>Add entity and trigger SaveChangesAsync.</summary>
    Task<TEntity> AddAsync(TEntity entity, CancellationToken ct = default);

    /// <summary>Update entity and trigger SaveChangesAsync.</summary>
    Task UpdateAsync(TEntity entity, CancellationToken ct = default);

    /// <summary>Remove entity and trigger SaveChangesAsync.</summary>
    Task DeleteAsync(TEntity entity, CancellationToken ct = default);

    /// <summary>Delete entity by ID and trigger SaveChangesAsync.</summary>
    Task DeleteByIdAsync(Guid id, CancellationToken ct = default);
}
