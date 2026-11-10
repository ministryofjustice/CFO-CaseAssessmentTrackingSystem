namespace Cfo.Cats.Application.Features.HmppsExternalApi.DTOs;

/// <summary>
/// Prison and community offender managers for a person, as returned by the HMPPS External API's
/// GET /v1/persons/{hmppsId}/person-responsible-officer endpoint.
/// </summary>
public class PersonResponsibleOfficerDto
{
    public PrisonOffenderManagerDto? PrisonOffenderManager { get; init; }
    public CommunityOffenderManagerDto? CommunityOffenderManager { get; init; }
}

public class PrisonOffenderManagerDto
{
    public string? Forename { get; init; }
    public string? Surname { get; init; }
    public PrisonDto? Prison { get; init; }
}

public class PrisonDto
{
    public string? Code { get; init; }
}

public class CommunityOffenderManagerDto
{
    public OfficerNameDto? Name { get; init; }
    public string? Email { get; init; }
    public string? TelephoneNumber { get; init; }
    public OfficerTeamDto? Team { get; init; }
}

public class OfficerNameDto
{
    public string? Forename { get; init; }
    public string? Surname { get; init; }
}

public class OfficerTeamDto
{
    public string? Code { get; init; }
    public string? Description { get; init; }
    public string? Email { get; init; }
    public string? TelephoneNumber { get; init; }
    public CodeDescriptionDto? Provider { get; init; }
}
