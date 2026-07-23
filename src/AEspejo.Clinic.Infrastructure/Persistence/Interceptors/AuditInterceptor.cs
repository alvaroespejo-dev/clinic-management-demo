using System.Text.Json;
using AEspejo.Clinic.Application.Interfaces;
using AEspejo.Clinic.Domain.Common;
using AEspejo.Clinic.Domain.Entities;
using AEspejo.Clinic.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace AEspejo.Clinic.Infrastructure.Persistence.Interceptors;

/// <summary>
/// EF Core interceptor that:
///  1. Maintains CreatedAt/UpdatedAt on BaseEntity entities.
///  2. Writes an AuditLog record for every Created/Updated/Deleted.
/// </summary>
public class AuditInterceptor(ICurrentUserService currentUser) : SaveChangesInterceptor
{

    public override InterceptionResult<int> SavingChanges(
        DbContextEventData eventData, InterceptionResult<int> result)
    {
        Apply(eventData.Context);
        return base.SavingChanges(eventData, result);
    }

    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(
        DbContextEventData eventData, InterceptionResult<int> result,
        CancellationToken cancellationToken = default)
    {
        Apply(eventData.Context);
        return base.SavingChangesAsync(eventData, result, cancellationToken);
    }

    private void Apply(DbContext? context)
    {
        if (context is null) return;

        var now = DateTimeOffset.UtcNow;
        List<AuditLog> auditEntries = [];

        foreach (var entry in context.ChangeTracker.Entries())
        {
            // Do not audit the audit table itself.
            if (entry.Entity is AuditLog) continue;

            // Automatic timestamps for business entities.
            if (entry.Entity is BaseEntity baseEntity)
            {
                if (entry.State == EntityState.Added) baseEntity.CreatedAt = now;
                if (entry.State == EntityState.Modified) baseEntity.UpdatedAt = now;
            }

            var action = entry.State switch
            {
                EntityState.Added => (AuditAction?)AuditAction.Created,
                EntityState.Modified => AuditAction.Updated,
                EntityState.Deleted => AuditAction.Deleted,
                _ => null
            };
            if (action is null) continue;

            auditEntries.Add(new AuditLog
            {
                TableName = entry.Metadata.GetTableName() ?? entry.Metadata.ClrType.Name,
                RecordId = GetPrimaryKey(entry),
                Action = action.Value,
                OldValues = action == AuditAction.Created ? null : Serialize(entry, original: true),
                NewValues = action == AuditAction.Deleted ? null : Serialize(entry, original: false),
                UserId = currentUser.UserId,
                IpAddress = currentUser.IpAddress,
                ChangedAt = now
            });
        }

        if (auditEntries.Count > 0)
            context.Set<AuditLog>().AddRange(auditEntries);
    }

    private static string GetPrimaryKey(EntityEntry entry)
    {
        var key = entry.Metadata.FindPrimaryKey();
        if (key is null) return string.Empty;
        var values = key.Properties.Select(p => entry.Property(p.Name).CurrentValue?.ToString());
        return string.Join(",", values);
    }

    private static string Serialize(EntityEntry entry, bool original)
    {
        var dict = entry.Properties.ToDictionary(
            p => p.Metadata.Name,
            p => original ? p.OriginalValue : p.CurrentValue);
        return JsonSerializer.Serialize(dict);
    }
}
