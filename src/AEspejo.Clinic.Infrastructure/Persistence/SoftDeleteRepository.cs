using AEspejo.Clinic.Application.Interfaces;
using AEspejo.Clinic.Domain.Common;

namespace AEspejo.Clinic.Infrastructure.Persistence;

/// <summary>
/// Repository for soft-delete entities (ISoftDeletable): Delete sets IsActive = false
/// instead of removing the row. Query() does NOT filter out inactive records — list filtering
/// is decided by the service (SoftDeleteCrudServiceBase.ApplyListFilter honours IncludeInactive)
/// and GetByIdAsync must be able to find inactive records so they can be reactivated or edited.
/// </summary>
public class SoftDeleteRepository<TEntity>(IAppDbContext context) : Repository<TEntity>(context)
    where TEntity : BaseEntity, ISoftDeletable
{
    public override async Task DeleteAsync(TEntity entity, CancellationToken ct = default)
    {
        entity.IsActive = false;
        await UpdateAsync(entity, ct);
    }
}
