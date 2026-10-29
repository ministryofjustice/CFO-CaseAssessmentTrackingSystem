namespace Cfo.Cats.Application.Features.QualityAssurance.DTOs;

public class QaOfficerStatsDto
{
    /// Counts grouped in the database, used by the charts and the summary table.
    public List<QaOfficerStatSummaryDto> Summary { get; set; } = [];

    /// One page of detail rows, used by the table.
    public PaginatedData<QaOfficerStatDto>? Rows { get; set; }
}

public class QaOfficerStatSummaryDto
{
    public DateTime Day { get; set; }
    public string Queue { get; set; } = string.Empty;
    public string? QaUser { get; set; }
    public int Count { get; set; }
}
