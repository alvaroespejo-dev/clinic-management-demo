using AEspejo.Clinic.Application.Interfaces;
using AEspejo.Clinic.Application.Repositories;
using AEspejo.Clinic.Domain.Entities;
using AEspejo.Clinic.Infrastructure.MultiTenancy;
using AEspejo.Clinic.Infrastructure.Persistence;
using AEspejo.Clinic.Infrastructure.Persistence.Interceptors;
using AEspejo.Clinic.Master;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace AEspejo.Clinic.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services, string masterConnectionString, DatabaseProvider provider)
    {
        // The selected engine is app-global; AppDbContext and services resolve it via DI.
        services.AddSingleton(new DatabaseProviderAccessor(provider));

        // Master database (company catalog).
        services.AddDbContext<MasterDbContext>(options =>
            options.UseConfiguredDatabase(provider, masterConnectionString));

        // Tenant context (scoped): provides ITenantProvider + ITenantConnectionResolver.
        services.AddScoped<TenantContext>();
        services.AddScoped<ITenantProvider>(sp => sp.GetRequiredService<TenantContext>());
        services.AddScoped<ITenantConnectionResolver>(sp => sp.GetRequiredService<TenantContext>());

        // Audit interceptor (depends on ICurrentUserService, registered in the API).
        services.AddScoped<AuditInterceptor>();

        // AppDbContext: the connection string is resolved by OnConfiguring via ITenantConnectionResolver.
        services.AddDbContext<AppDbContext>((sp, options) =>
            options.AddInterceptors(sp.GetRequiredService<AuditInterceptor>()));
        services.AddScoped<IAppDbContext>(sp => sp.GetRequiredService<AppDbContext>());

        // Per-tenant database provisioning.
        services.AddScoped<TenantProvisioningService>();

        // Repositories
        AddRepositories(services);

        return services;
    }

    private static void AddRepositories(IServiceCollection services)
    {
        // Generic repository (physical deletion) for any entity.
        services.AddScoped(typeof(IRepository<>), typeof(Repository<>));

        // ISoftDeletable entities: the closed registration wins over the open generic,
        // so DeleteAsync sets IsActive = false instead of removing the row.
        services.AddScoped<IRepository<User>, SoftDeleteRepository<User>>();
        services.AddScoped<IRepository<Branch>, SoftDeleteRepository<Branch>>();
        services.AddScoped<IRepository<Patient>, SoftDeleteRepository<Patient>>();
        services.AddScoped<IRepository<Room>, SoftDeleteRepository<Room>>();
        services.AddScoped<IRepository<MedicalCondition>, SoftDeleteRepository<MedicalCondition>>();
        services.AddScoped<IRepository<ServiceCatalog>, SoftDeleteRepository<ServiceCatalog>>();
    }
}
