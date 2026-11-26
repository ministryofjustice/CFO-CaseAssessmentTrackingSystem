namespace Cfo.Cats.Application.Features.HmppsExternalApi.DTOs;

/// <summary>
/// Licences and their conditions for a person, as returned by the HMPPS External API's
/// GET /v1/persons/{hmppsId}/licences/conditions endpoint. Sourced from Create and Vary a Licence (CVL) using the CRN.
/// </summary>
public class PersonLicencesDto
{
    public string? HmppsId { get; init; }
    public string? OffenderNumber { get; init; }
    public List<LicenceDto>? Licences { get; init; }
}

public class LicenceDto
{
    /// <summary>
    /// e.g. "ACTIVE", "IN_PROGRESS", "SUBMITTED", "APPROVED", "VARIATION_APPROVED".
    /// </summary>
    public string? Status { get; init; }

    /// <summary>
    /// "AP" (licence), "PSS" (post sentence supervision) or "AP_PSS" (both).
    /// </summary>
    public string? TypeCode { get; init; }
    public string? CreatedDate { get; init; }
    public string? ApprovedDate { get; init; }
    public string? UpdatedDate { get; init; }
    public List<LicenceConditionDto>? Conditions { get; init; }
}

public class LicenceConditionDto
{
    public string? Type { get; init; }
    public string? Code { get; init; }
    public string? Category { get; init; }

    /// <summary>
    /// Condition wording. Masked as <see cref="RedactedText"/> for Limited Access Offenders.
    /// </summary>
    public string? Condition { get; init; }

    public const string RedactedText = "**REDACTED**";

    public bool IsRedacted => Condition == RedactedText;
}
