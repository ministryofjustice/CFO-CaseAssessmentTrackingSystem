using System.Reflection;

namespace Cfo.Cats.Server.UI.Services.Navigation;

/// <summary>
/// Maps an absolute path to the page component that serves it, using the same @page routes the router uses.
/// </summary>
public static class PageRoutes
{
    private sealed record Route(string[] Segments, Type Page);

    private static readonly Lazy<IReadOnlyList<Route>> Routes = new(() => typeof(PageRoutes).Assembly
        .GetTypes()
        .Where(t => typeof(IComponent).IsAssignableFrom(t))
        .SelectMany(t => t.GetCustomAttributes<RouteAttribute>().Select(r => new Route(Split(r.Template), t)))
        .ToList());

    public static Type? Resolve(string path)
    {
        var segments = Split(path);

        return Routes.Value
            .Where(r => Matches(r.Segments, segments))
            .OrderByDescending(r => r.Segments.Count(s => !IsParameter(s)))
            .ThenByDescending(r => r.Segments.Count(HasConstraint))
            .Select(r => r.Page)
            .FirstOrDefault();
    }

    private static bool Matches(string[] template, string[] path)
    {
        for (var i = 0; i < template.Length; i++)
        {
            if (template[i].StartsWith("{*", StringComparison.Ordinal))
            {
                return true;
            }

            if (i >= path.Length)
            {
                return template[i].EndsWith("?}", StringComparison.Ordinal) && template.Skip(i).All(s => s.EndsWith("?}", StringComparison.Ordinal));
            }

            if (IsParameter(template[i]))
            {
                if (!MatchesConstraint(template[i], path[i]))
                {
                    return false;
                }

                continue;
            }

            if (!template[i].Equals(path[i], StringComparison.OrdinalIgnoreCase))
            {
                return false;
            }
        }

        return template.Length == path.Length;
    }

    private static bool MatchesConstraint(string parameterSegment, string value)
    {
        var inner = parameterSegment.AsSpan(1, parameterSegment.Length - 2).TrimEnd('?');
        var colonIndex = inner.IndexOf(':');
        if (colonIndex < 0)
        {
            return true;
        }

        var constraint = inner[(colonIndex + 1)..];
        return constraint switch
        {
            "guid" => Guid.TryParse(value, out _),
            "int" => int.TryParse(value, System.Globalization.NumberStyles.Integer, System.Globalization.CultureInfo.InvariantCulture, out _),
            "long" => long.TryParse(value, System.Globalization.NumberStyles.Integer, System.Globalization.CultureInfo.InvariantCulture, out _),
            "bool" => bool.TryParse(value, out _),
            _ => true
        };
    }

    private static bool HasConstraint(string segment) => IsParameter(segment) && segment.Contains(':', StringComparison.Ordinal);

    private static bool IsParameter(string segment) => segment.StartsWith('{') && segment.EndsWith('}');

    private static string[] Split(string value) => value.Split('/', StringSplitOptions.RemoveEmptyEntries);
}
