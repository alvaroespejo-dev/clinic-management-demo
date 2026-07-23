using AEspejo.Clinic.Application.Auth;
using AEspejo.Clinic.Domain.Entities;
using AEspejo.Clinic.Domain.Enums;
using AEspejo.Clinic.Infrastructure.Persistence;
using AEspejo.Clinic.Master.Entities;
using Microsoft.EntityFrameworkCore;

namespace AEspejo.Clinic.Infrastructure.MultiTenancy;

/// <summary>
/// Provisions a tenant database: creates it (when missing), applies the clinical model
/// migrations and seeds baseline data (one branch + one administrator user).
/// </summary>
public class TenantProvisioningService(DatabaseProviderAccessor providerAccessor)
{
    private readonly DatabaseProvider _provider = providerAccessor.Provider;

    /// <summary>Creates/migrates the tenant database and seeds admin+branch. Returns the password used (if one was created).</summary>
    public async Task ProvisionAsync(Tenant tenant, string adminEmail, string adminPassword, CancellationToken ct = default)
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseConfiguredDatabase(_provider, tenant.ConnectionString)
            .Options;

        await using var ctx = new AppDbContext(options);
        await ctx.EnsureDatabaseAsync(_provider, ct);

        if (!await ctx.Users.AnyAsync(ct))
        {
            var now = DateTimeOffset.UtcNow;

            var branch = new Branch
            {
                Name = tenant.CompanyName,
                Address = string.Empty,
                Phone = string.Empty,
                Email = tenant.ContactEmail,
                IsActive = true,
                CreatedAt = now
            };
            ctx.Branches.Add(branch);

            ctx.Users.Add(new User
            {
                Email = adminEmail,
                PasswordHash = PasswordHasher.Hash(adminPassword),
                FirstName = "Admin",
                LastName = tenant.CompanyName,
                Role = UserRole.Admin,
                PreferredLanguage = tenant.DefaultLanguage,
                BranchId = branch.Id,
                IsActive = true,
                CreatedAt = now
            });

            // Initialize organization configuration with tenant company name
            ctx.OrgConfigs.Add(new OrgConfig
            {
                Id = new Guid("00000000-0000-0000-0000-000000000001"),
                Name = tenant.CompanyName,
                CreatedAt = now.DateTime,
                UpdatedAt = now.DateTime
            });

            await ctx.SaveChangesAsync(ct);
        }
    }
}
