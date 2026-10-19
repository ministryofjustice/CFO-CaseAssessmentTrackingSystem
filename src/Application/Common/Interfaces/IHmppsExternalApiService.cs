using Cfo.Cats.Application.Features.HmppsExternalApi.DTOs;

namespace Cfo.Cats.Application.Common.Interfaces;

/// <summary>
/// Client for the HMPPS External API, authenticated via mutual TLS and an API key.
/// </summary>
public interface IHmppsExternalApiService
{
    /// <summary>
    /// Calls the API's versioned status endpoint. Useful for verifying that the mTLS handshake and
    /// API key are configured correctly end-to-end.
    /// </summary>
    Task<Result<string>> GetStatusAsync(CancellationToken cancellationToken);

    /// <summary>
    /// Returns Risk of Serious Harm (ROSH) risks associated with a person, based on assessments completed
    /// in the last year. Does not serve LAO (Limited Access Offender) data.
    /// </summary>
    /// <param name="hmppsId">HMPPS identifier for the person (e.g. NOMIS number).</param>
    Task<Result<RisksDto>> GetSeriousHarmRiskAsync(string hmppsId, CancellationToken cancellationToken);

    /// <summary>
    /// Returns the core person record for a person.
    /// </summary>
    /// <param name="hmppsId">HMPPS identifier for the person (e.g. NOMIS number).</param>
    Task<Result<PersonDto>> GetPersonAsync(string hmppsId, CancellationToken cancellationToken);

    /// <summary>
    /// Returns the latest tier calculation for a person, including the Delius and OASys inputs used to calculate it.
    /// </summary>
    /// <param name="hmppsId">HMPPS identifier for the person (e.g. CRN).</param>
    Task<Result<TierDetailsDto>> GetTierDetailsAsync(string hmppsId, CancellationToken cancellationToken);
}
