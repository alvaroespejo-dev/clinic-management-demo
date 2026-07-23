namespace AEspejo.Clinic.Application.Dtos;

public class OrgConfigDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? LogoUrl { get; set; }
}

public class UpdateOrgConfigDto
{
    public string Name { get; set; } = string.Empty;
    public string? LogoUrl { get; set; }
}
