namespace Cfo.Cats.Application.Features.HmppsExternalApi.DTOs;

/// <summary>
/// A sentence for a person, as returned by the HMPPS External API's GET /v1/persons/{hmppsId}/sentences endpoint.
/// </summary>
public class SentenceDto : IHasSystemSource
{
    /// <summary>
    /// "PRISON_API" or "NDELIUS".
    /// </summary>
    public string? ServiceSource { get; init; }

    /// <summary>
    /// "PRISON_SYSTEMS" or "PROBATION_SYSTEMS". See <see cref="HmppsSystemSource"/>.
    /// </summary>
    public string? SystemSource { get; init; }
    public DateOnly? DateOfSentencing { get; init; }
    public string? Description { get; init; }
    public bool? IsActive { get; init; }
    public bool? IsCustodial { get; init; }
    public decimal? FineAmount { get; init; }
    public SentenceLengthDto? Length { get; init; }
}

/// <summary>
/// Probation sentences populate <see cref="Duration"/> and <see cref="Units"/>; prison sentences populate <see cref="Terms"/>.
/// </summary>
public class SentenceLengthDto
{
    public int? Duration { get; init; }

    /// <summary>
    /// "Hours", "Days", "Weeks", "Months" or "Years".
    /// </summary>
    public string? Units { get; init; }
    public List<SentenceTermDto>? Terms { get; init; }
}

public class SentenceTermDto
{
    public int? Years { get; init; }
    public int? Months { get; init; }
    public int? Weeks { get; init; }
    public int? Days { get; init; }
    public int? Hours { get; init; }

    /// <summary>
    /// e.g. "IMP" (Imprisonment), "LIC" (Licence), "DET" (Detention). See the HMPPS External API documentation.
    /// </summary>
    public string? PrisonTermCode { get; init; }
}
