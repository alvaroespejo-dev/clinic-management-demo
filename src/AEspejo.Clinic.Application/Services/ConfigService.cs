using AEspejo.Clinic.Application.Dtos;
using AEspejo.Clinic.Application.Interfaces;
using AEspejo.Clinic.Domain.Entities;
using Mapster;
using Microsoft.EntityFrameworkCore;

namespace AEspejo.Clinic.Application.Services;

/// <summary>
/// Service for managing organization configuration (clinic name, logo, etc.).
/// </summary>
public class ConfigService(IAppDbContext db)
{
    private readonly IAppDbContext _db = db;
    private const string DefaultConfigId = "00000000-0000-0000-0000-000000000001";

    /// <summary>
    /// Retrieves the organization configuration. Creates a default one if it doesn't exist.
    /// </summary>
    public async Task<OrgConfigDto> GetConfigAsync(CancellationToken ct = default)
    {
        var configId = new Guid(DefaultConfigId);
        var config = await _db.OrgConfigs.FirstOrDefaultAsync(c => c.Id == configId, ct);
        if (config == null)
        {
            config = new OrgConfig
            {
                Id = configId,
                Name = "My Clinic",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
            };
            _db.OrgConfigs.Add(config);
            await _db.SaveChangesAsync(ct);
        }
        return config.Adapt<OrgConfigDto>();
    }

    /// <summary>
    /// Updates the organization configuration with provided data.
    /// </summary>
    public async Task<OrgConfigDto> UpdateConfigAsync(UpdateOrgConfigDto dto, CancellationToken ct = default)
    {
        var configId = new Guid(DefaultConfigId);
        var config = await _db.OrgConfigs.FirstOrDefaultAsync(c => c.Id == configId, ct);
        if (config == null)
        {
            config = new OrgConfig
            {
                Id = configId,
                CreatedAt = DateTime.UtcNow,
            };
            _db.OrgConfigs.Add(config);
        }

        config.Name = dto.Name;
        config.LogoUrl = dto.LogoUrl;
        config.UpdatedAt = DateTime.UtcNow;

        await _db.SaveChangesAsync(ct);
        return config.Adapt<OrgConfigDto>();
    }
}
