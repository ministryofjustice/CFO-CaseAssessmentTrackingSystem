namespace Cfo.Cats.Application.Features.HmppsExternalApi.DTOs;

/// <summary>
/// Offender managers responsible for a person, as returned by the HMPPS External API's
/// GET /v1/persons/{hmppsId}/person-responsible-officer endpoint.
/// </summary>
public class ResponsibleOfficerDto
{
    public OffenderManagerDto? PrisonOffenderManager { get; init; }
    public OffenderManagerDto? CommunityOffenderManager { get; init; }
}

public class OffenderManagerDto
{
    public OffenderManagerNameDto? Name { get; init; }
    public string? Email { get; init; }
    public string? TelephoneNumber { get; init; }
    public OffenderManagerTeamDto? Team { get; init; }
}

public class OffenderManagerNameDto
{
    public string? Forename { get; init; }
    public string? Surname { get; init; }
}

public class OffenderManagerTeamDto
{
    public string? Code { get; init; }
    public string? Description { get; init; }
    public string? Email { get; init; }
    public string? TelephoneNumber { get; init; }
    public CodeDescriptionDto? Provider { get; init; }
}
