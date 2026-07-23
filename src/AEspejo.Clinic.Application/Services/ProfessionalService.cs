using AEspejo.Clinic.Application.Common;
using AEspejo.Clinic.Application.Dtos;
using AEspejo.Clinic.Application.Interfaces;
using AEspejo.Clinic.Application.Repositories;
using AEspejo.Clinic.Domain.Entities;
using FluentValidation;
using Mapster;
using Microsoft.EntityFrameworkCore;

namespace AEspejo.Clinic.Application.Services;

/// <summary>
/// Professional shares its PK with User (1:1). On create, Id = UserId; it is not auto-generated.
/// </summary>
public class ProfessionalService(IAppDbContext db, IRepository<Professional> repo,
    IValidator<CreateProfessionalDto>? cv = null, IValidator<UpdateProfessionalDto>? uv = null)
    : CrudServiceBase<Professional, CreateProfessionalDto, UpdateProfessionalDto, ProfessionalDto>(db, repo, cv, uv)
{
    // Includes the user so FirstName and LastName can be displayed (Mapster flattens User.FirstName/LastName → FirstName/LastName).
    protected override IQueryable<Professional> BaseQuery() => Repository.Query().Include(p => p.User);

    public override async Task<Result<ProfessionalDto>> CreateAsync(CreateProfessionalDto dto, CancellationToken ct = default)
    {
        var userExists = await Db.Users.AnyAsync(u => u.Id == dto.UserId, ct);
        if (!userExists)
            return Result<ProfessionalDto>.Invalid("El UserId indicado no existe.");

        var exists = await Repository.Query().AnyAsync(p => p.Id == dto.UserId, ct);
        if (exists)
            return Result<ProfessionalDto>.Conflict("Ese usuario ya es un profesional.");

        var professional = new Professional
        {
            Id = dto.UserId,
            LicenseNumber = dto.LicenseNumber,
            Specialty = dto.Specialty,
            Color = dto.Color
        };
        await Repository.AddAsync(professional, ct);

        // Reload the entity with the User relationship included so Mapster can map FirstName/LastName
        var result = await Repository.Query().Include(p => p.User).FirstOrDefaultAsync(p => p.Id == professional.Id, ct);
        return Result<ProfessionalDto>.Ok(result!.Adapt<ProfessionalDto>());
    }
}
