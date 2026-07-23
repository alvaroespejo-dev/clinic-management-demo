using AEspejo.Clinic.Application.Common;
using AEspejo.Clinic.Application.Dtos;
using AEspejo.Clinic.Application.Interfaces;
using Mapster;
using Microsoft.EntityFrameworkCore;

namespace AEspejo.Clinic.Application.Services;

/// <summary>Audit log: READ-ONLY. Records are immutable (no create/update/delete through the API).</summary>
public interface IAuditLogService
{
    Task<PagedResult<AuditLogDto>> ListAsync(PagedRequest request, CancellationToken ct = default);
    Task<Result<AuditLogDto>> GetByIdAsync(long id, CancellationToken ct = default);
}

public class AuditLogService(IAppDbContext db) : IAuditLogService
{
    private readonly IAppDbContext _db = db;

    public async Task<PagedResult<AuditLogDto>> ListAsync(PagedRequest request, CancellationToken ct = default)
    {
        var query = _db.AuditLogs.AsQueryable();
        if (!string.IsNullOrWhiteSpace(request.Search))
        {
            var s = request.Search.Trim();
            query = query.Where(a => a.TableName.Contains(s) || a.RecordId.Contains(s));
        }

        var total = await query.CountAsync(ct);
        var items = await query
            .OrderByDescending(a => a.ChangedAt)
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToListAsync(ct);

        return new PagedResult<AuditLogDto>
        {
            Items = items.Adapt<List<AuditLogDto>>(),
            TotalCount = total,
            Page = request.Page,
            PageSize = request.PageSize
        };
    }

    public async Task<Result<AuditLogDto>> GetByIdAsync(long id, CancellationToken ct = default)
    {
        var entity = await _db.AuditLogs.FirstOrDefaultAsync(a => a.Id == id, ct);
        return entity is null ? Result<AuditLogDto>.NotFound() : Result<AuditLogDto>.Ok(entity.Adapt<AuditLogDto>());
    }
}
