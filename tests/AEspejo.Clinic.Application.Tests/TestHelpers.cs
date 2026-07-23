using AEspejo.Clinic.Application.Common;
using AEspejo.Clinic.Application.Interfaces;
using AEspejo.Clinic.Application.Repositories;
using AEspejo.Clinic.Domain.Common;
using AEspejo.Clinic.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace AEspejo.Clinic.Application.Tests;

/// <summary>
/// Fake current-user service for unit tests: returns a fixed user id and no IP address.
/// </summary>
public sealed class FakeCurrentUser(Guid? userId = null) : ICurrentUserService
{
    public Guid? UserId { get; } = userId ?? Guid.NewGuid();
    public string? IpAddress => null;
}

/// <summary>
/// Builds an isolated in-memory <see cref="AppDbContext"/> (one database per test) plus a
/// <see cref="Repository{TEntity}"/> over it. Registers Mapster mappings once for the whole run.
/// </summary>
public static class TestDb
{
    static TestDb() => MapsterConfig.RegisterMappings();

    public static AppDbContext NewContext()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase($"clinic-tests-{Guid.NewGuid()}")
            .Options;
        // Resolver is null: options already carry a provider, so OnConfiguring is a no-op.
        return new AppDbContext(options);
    }

    public static IRepository<TEntity> RepositoryFor<TEntity>(AppDbContext ctx)
        where TEntity : BaseEntity => new Repository<TEntity>(ctx);
}
