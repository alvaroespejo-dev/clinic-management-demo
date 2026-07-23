using AEspejo.Clinic.Application.Common;
using AEspejo.Clinic.Application.Dtos;
using AEspejo.Clinic.Application.Interfaces;
using AEspejo.Clinic.Application.Repositories;
using AEspejo.Clinic.Domain.Entities;
using FluentValidation;
using Mapster;

namespace AEspejo.Clinic.Application.Services;

/// <summary>Records UpdatedByUserId with the request's user on create/update.</summary>
public class ToothRecordService(IAppDbContext db, IRepository<ToothRecord> repo, ICurrentUserService currentUser,
    IValidator<CreateToothRecordDto>? cv = null, IValidator<UpdateToothRecordDto>? uv = null)
    : CrudServiceBase<ToothRecord, CreateToothRecordDto, UpdateToothRecordDto, ToothRecordDto>(db, repo, cv, uv)
{
    public override async Task<Result<ToothRecordDto>> CreateAsync(CreateToothRecordDto dto, CancellationToken ct = default)
    {
        var entity = dto.Adapt<ToothRecord>();
        entity.UpdatedByUserId = currentUser.UserId ?? Guid.Empty;
        await Repository.AddAsync(entity, ct);
        return Result<ToothRecordDto>.Ok(entity.Adapt<ToothRecordDto>());
    }

    public override async Task<Result<ToothRecordDto>> UpdateAsync(Guid id, UpdateToothRecordDto dto, CancellationToken ct = default)
    {
        var entity = await Repository.GetByIdAsync(id, ct);
        if (entity is null) return Result<ToothRecordDto>.NotFound();

        dto.Adapt(entity);
        entity.UpdatedByUserId = currentUser.UserId ?? entity.UpdatedByUserId;
        await Repository.UpdateAsync(entity, ct);
        return Result<ToothRecordDto>.Ok(entity.Adapt<ToothRecordDto>());
    }
}
