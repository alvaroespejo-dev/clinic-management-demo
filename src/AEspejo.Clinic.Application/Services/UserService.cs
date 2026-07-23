using AEspejo.Clinic.Application.Auth;
using AEspejo.Clinic.Application.Common;
using AEspejo.Clinic.Application.Dtos;
using AEspejo.Clinic.Application.Interfaces;
using AEspejo.Clinic.Application.Repositories;
using AEspejo.Clinic.Domain.Entities;
using FluentValidation;
using Mapster;
using Microsoft.EntityFrameworkCore;

namespace AEspejo.Clinic.Application.Services;

public class UserService(IAppDbContext db, IRepository<User> repo,
    IValidator<CreateUserDto>? cv = null, IValidator<UpdateUserDto>? uv = null)
    : SoftDeleteCrudServiceBase<User, CreateUserDto, UpdateUserDto, UserDto>(db, repo, cv, uv)
{
    protected override IQueryable<User> ApplySearch(IQueryable<User> query, string search) =>
        query.Where(u => u.Email.Contains(search) || u.FirstName.Contains(search) || u.LastName.Contains(search));

    public override async Task<Result<UserDto>> CreateAsync(CreateUserDto dto, CancellationToken ct = default)
    {
        var emailTaken = await Repository.Query().AnyAsync(u => u.Email == dto.Email, ct);
        if (emailTaken)
            return Result<UserDto>.Conflict("Ya existe un usuario con ese email.");

        var user = dto.Adapt<User>();
        user.PasswordHash = PasswordHasher.Hash(dto.Password);
        await Repository.AddAsync(user, ct);
        return Result<UserDto>.Ok(user.Adapt<UserDto>());
    }
}
