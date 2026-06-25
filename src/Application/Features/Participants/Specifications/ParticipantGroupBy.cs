namespace Cfo.Cats.Application.Features.Participants.Specifications;

public enum ParticipantGroupBy
{
    [Description("None")] None,
    [Description("Assignee")] Assignee,
    [Description("Tenant")] Tenant
}

public static class ParticipantGroupByExtensions
{
    public static string GetDescription(this ParticipantGroupBy e)
        => e switch
        {
            ParticipantGroupBy.None => "None",
            ParticipantGroupBy.Assignee => "Assignee",
            ParticipantGroupBy.Tenant => "Tenant",
            _ => throw new ArgumentOutOfRangeException(nameof(e), "Unknown group by type")
        };
}
