namespace Cfo.Cats.Application.Features.HmppsExternalApi.DTOs;

/// <summary>
/// An offence for a person, as returned by the HMPPS External API's GET /v1/persons/{hmppsId}/offences endpoint.
/// </summary>
public class OffenceDto : IHasSystemSource
{
    /// <summary>
    /// "PRISON_API" or "NDELIUS".
    /// </summary>
    public string? ServiceSource { get; init; }

    /// <summary>
    /// "PRISON_SYSTEMS" or "PROBATION_SYSTEMS". See <see cref="HmppsSystemSource"/>.
    /// </summary>
    public string? SystemSource { get; init; }

    /// <summary>
    /// Criminal Justice Standard offence code. Populated for prison offences.
    /// </summary>
    public string? CjsCode { get; init; }

    /// <summary>
    /// Home Office offence code. Populated for probation offences.
    /// </summary>
    public string? HoCode { get; init; }
    public List<DateOnly?>? CourtDates { get; init; }
    public string? CourtName { get; init; }
    public string? Description { get; init; }
    public DateOnly? StartDate { get; init; }
    public DateOnly? EndDate { get; init; }
    public string? StatuteCode { get; init; }
}
