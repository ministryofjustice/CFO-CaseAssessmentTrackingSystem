namespace Cfo.Cats.Application.Features.HmppsExternalApi.DTOs;

/// <summary>
/// Values of the <c>systemSource</c> field the HMPPS External API attaches to merged records.
/// </summary>
/// <remarks>
/// Whether called with a NOMIS number or a CRN, the API resolves the person and returns data from
/// both prison and probation systems. Filter on this value to show only the data owned by the record queried.
/// </remarks>
public static class HmppsSystemSource
{
    public const string PrisonSystems = "PRISON_SYSTEMS";
    public const string ProbationSystems = "PROBATION_SYSTEMS";

    /// <summary>
    /// Keeps only items from <paramref name="systemSource"/>, preserving not-found and failure results unchanged.
    /// </summary>
    public static Result<T[]> FromSystem<T>(this Result<T[]> result, string systemSource)
        where T : IHasSystemSource
    {
        if (result is NotFoundResult<T[]> || !result.Succeeded || result.Data is null)
        {
            return result;
        }

        return Result<T[]>.Success(result.Data.Where(item => item.SystemSource == systemSource).ToArray());
    }
}

public interface IHasSystemSource
{
    string? SystemSource { get; }
}
