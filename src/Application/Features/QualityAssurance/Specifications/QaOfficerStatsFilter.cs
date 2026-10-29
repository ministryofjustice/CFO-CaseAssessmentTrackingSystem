namespace Cfo.Cats.Application.Features.QualityAssurance.Specifications;

public enum QaStatsProcess
{
    Enrolments,
    Activities
}

public enum QaStatsQueue
{
    Qa1,
    Qa2,
    Escalation
}

public class QaOfficerStatsFilter
{
    /// Longest date range, in months, that can be requested, to stop too much data being returned at once.
    public const int MaxRangeMonths = 3;

    public static readonly string MaxRangeMessage = $"The date range cannot exceed {MaxRangeMonths} months.";

    public QaStatsProcess Process { get; set; }

    public DateTime StartDate { get; set; } = DateTime.Today;

    public DateTime EndDate { get; set; } = DateTime.Today;

    public bool IsWithinMaxRange() => EndDate.Date <= StartDate.Date.AddMonths(MaxRangeMonths);

    public List<QaStatsQueue> Queues { get; set; } = [];

    public string? QaUserId { get; set; }

    public bool? IsAccepted { get; set; }

    public bool NotesOnly { get; set; }

    public string? Crn { get; set; }
}
