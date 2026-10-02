using System.Text.RegularExpressions;

namespace Cfo.Cats.Server.UI.Services.Help;

/// <summary>
/// Normalizes an absolute request path into a stable "page key" that can be stored against
/// a <see cref="Cfo.Cats.Domain.HelpLinks.HelpLink"/>, by replacing dynamic route segments
/// with a placeholder. This lets the same help link match every participant/entity that
/// shares the same page template, e.g.:
/// <c>/pages/workspace/participants/3fa85f64-5717-4562-b3fc-2c963f66afa6</c> normalizes to
/// <c>/pages/workspace/participants/{id}</c>.
/// </summary>
/// <remarks>
/// When the caller supplies the matched <c>RouteData.RouteValues</c> for the current
/// navigation (see <c>Routes.razor</c>'s <c>&lt;Found&gt;</c> template), any path segment whose
/// value equals one of those route values is replaced with <c>{paramName}</c> - this is the
/// preferred path, since it works for any route parameter regardless of its actual format
/// (guid, int, NOMIS number, CRN, etc.), because it relies on ASP.NET's own routing match
/// rather than guessing from the segment's shape.
/// A guid/numeric regex is kept as a fallback for when route values aren't available yet
/// (e.g. before the router's first render).
/// </remarks>
public static partial class HelpPageKeyNormalizer
{
    public static string Normalize(string absolutePath, IReadOnlyDictionary<string, object?>? routeValues = null)
    {
        var path = absolutePath.Trim('/').ToLowerInvariant();

        if (path.Length == 0)
        {
            return "/";
        }

        var segments = path.Split('/', StringSplitOptions.RemoveEmptyEntries);

        for (var i = 0; i < segments.Length; i++)
        {
            var routeParamName = FindMatchingRouteParamName(segments[i], routeValues);

            segments[i] = routeParamName is not null
                ? $"{{{routeParamName}}}"
                : IsDynamicSegment(segments[i]) ? "{id}" : segments[i];
        }

        return "/" + string.Join('/', segments);
    }

    private static string? FindMatchingRouteParamName(string segment, IReadOnlyDictionary<string, object?>? routeValues)
    {
        if (routeValues is null)
        {
            return null;
        }

        foreach (var (key, value) in routeValues)
        {
            if (value is not null && string.Equals(value.ToString(), segment, StringComparison.OrdinalIgnoreCase))
            {
                return key.ToLowerInvariant();
            }
        }

        return null;
    }

    private static bool IsDynamicSegment(string segment)
        => GuidSegment().IsMatch(segment) || NumericSegment().IsMatch(segment);

    [GeneratedRegex("^[0-9a-f]{8}-[0-9a-f]{4}-[0-9a-f]{4}-[0-9a-f]{4}-[0-9a-f]{12}$")]
    private static partial Regex GuidSegment();

    [GeneratedRegex("^[0-9]+$")]
    private static partial Regex NumericSegment();
}
