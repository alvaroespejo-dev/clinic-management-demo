using AEspejo.Clinic.Application.Dtos;
using AEspejo.Clinic.Domain.Entities;
using Mapster;

namespace AEspejo.Clinic.Application.Common;

/// <summary>
/// Explicit Mapster mappings that the automatic flattening convention does not cover.
/// Mapster only flattens nested properties when the destination name follows the
/// "{NavProp}{Prop}" pattern (e.g. Branch.Name → BranchName, as in RoomDto). When the DTO uses
/// the short name (e.g. FirstName instead of UserFirstName), an explicit mapping is required.
/// </summary>
public static class MapsterConfig
{
    public static void RegisterMappings()
    {
        TypeAdapterConfig<Professional, ProfessionalDto>.NewConfig()
            .Map(dest => dest.FirstName, src => src.User.FirstName)
            .Map(dest => dest.LastName, src => src.User.LastName);
    }
}
