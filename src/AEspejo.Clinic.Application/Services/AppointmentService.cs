using AEspejo.Clinic.Application.Common;
using AEspejo.Clinic.Application.Dtos;
using AEspejo.Clinic.Application.Interfaces;
using AEspejo.Clinic.Application.Repositories;
using AEspejo.Clinic.Domain.Entities;
using AEspejo.Clinic.Domain.Enums;
using FluentValidation;
using Mapster;
using Microsoft.EntityFrameworkCore;

namespace AEspejo.Clinic.Application.Services;

/// <summary>
/// Records CreatedByUserId with the request's user and validates schedule overlap
/// for the same professional (non-cancelled appointments).
/// </summary>
public class AppointmentService(IAppDbContext db, IRepository<Appointment> repo, ICurrentUserService currentUser,
    IValidator<CreateAppointmentDto>? cv = null, IValidator<UpdateAppointmentDto>? uv = null)
    : CrudServiceBase<Appointment, CreateAppointmentDto, UpdateAppointmentDto, AppointmentDto>(db, repo, cv, uv)
{
    public override async Task<Result<AppointmentDto>> CreateAsync(CreateAppointmentDto dto, CancellationToken ct = default)
    {
        var conflict = await HasOverlapAsync(dto.ProfessionalId, dto.ScheduledAt, dto.DurationMinutes, null, ct);
        if (conflict)
            return Result<AppointmentDto>.Conflict("El profesional ya tiene una cita en ese horario.");

        var entity = dto.Adapt<Appointment>();
        entity.CreatedByUserId = currentUser.UserId ?? Guid.Empty;
        entity.Status = AppointmentStatus.Scheduled;
        await Repository.AddAsync(entity, ct);
        return Result<AppointmentDto>.Ok(entity.Adapt<AppointmentDto>());
    }

    public override async Task<Result<AppointmentDto>> UpdateAsync(Guid id, UpdateAppointmentDto dto, CancellationToken ct = default)
    {
        var entity = await Repository.GetByIdAsync(id, ct);
        if (entity is null) return Result<AppointmentDto>.NotFound();

        var conflict = await HasOverlapAsync(entity.ProfessionalId, dto.ScheduledAt, dto.DurationMinutes, id, ct);
        if (conflict)
            return Result<AppointmentDto>.Conflict("El profesional ya tiene una cita en ese horario.");

        dto.Adapt(entity);
        await Repository.UpdateAsync(entity, ct);
        return Result<AppointmentDto>.Ok(entity.Adapt<AppointmentDto>());
    }

    private async Task<bool> HasOverlapAsync(Guid professionalId, DateTimeOffset start, int durationMinutes, Guid? excludeId, CancellationToken ct)
    {
        var end = start.AddMinutes(durationMinutes);
        return await Repository.Query()
            .Where(a => a.ProfessionalId == professionalId
                        && a.Status != AppointmentStatus.Cancelled
                        && a.Status != AppointmentStatus.NoShow
                        && (excludeId == null || a.Id != excludeId))
            // overlaps if it starts before the new one ends and ends after the new one starts
            .AnyAsync(a => a.ScheduledAt < end
                        && start < a.ScheduledAt.AddMinutes(a.DurationMinutes), ct);
    }
}
