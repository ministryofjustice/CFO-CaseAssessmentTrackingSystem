namespace Cfo.Cats.Application.Common.Exports;

/// <summary>
/// Build filenames and descriptions for exported documents.
/// </summary>
public static class ExportDocumentNaming
{
    private const string Extension = ".xlsx";

    public static string BuildFileName(string exportName, DateTime? generatedOn = null)
    {
        var date = generatedOn ?? DateTime.UtcNow;
        return $"{Sanitize(exportName)}_{date:yyyy}-{date:MM}-{date:dd}_{date:HH}-{date:mm}{Extension}";
    }

    public static string BuildDescription(string baseDescription, params (string Label, string? Value)[] filters)
    {
        var appliedFilters = filters
            .Where(f => !string.IsNullOrWhiteSpace(f.Value))
            .Select(f => $"{f.Label} - {f.Value}")
            .ToList();

        return appliedFilters.Count == 0
            ? baseDescription
            : $"{baseDescription}. Filters applied:\n{string.Join("\n", appliedFilters)}";
    }

    private static string Sanitize(string value)
    {
        var invalidChars = Path.GetInvalidFileNameChars();
        var sanitized = new string(value.Select(c => invalidChars.Contains(c) ? '_' : c).ToArray());
        return sanitized.Replace(" ", "_");
    }
}
