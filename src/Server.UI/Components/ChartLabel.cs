using System.Collections.Concurrent;

namespace Cfo.Cats.Server.UI.Components;

public static class ChartLabel
{
    // Cached so identical labels share one instance; the chart groups by reference for array labels
    private static readonly ConcurrentDictionary<string, object> Cache = new();

    private const int MaxLineLength = 14;

    /// <summary>
    /// Wraps long labels onto a few short lines so rotated x-axis text isn't cropped.
    /// </summary>
    public static object Wrap(string? label)
        => Cache.GetOrAdd(label ?? string.Empty, static l =>
        {
            if (l.Length <= MaxLineLength)
            {
                return l;
            }

            var lines = new List<string>();
            var current = string.Empty;
            foreach (var word in l.Split(' ', StringSplitOptions.RemoveEmptyEntries))
            {
                if (current.Length > 0 && current.Length + 1 + word.Length > MaxLineLength)
                {
                    lines.Add(current);
                    current = word;
                }
                else
                {
                    current = current.Length == 0 ? word : $"{current} {word}";
                }
            }
            lines.Add(current);
            return lines.ToArray();
        });
}
