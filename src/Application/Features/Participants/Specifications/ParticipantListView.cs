namespace Cfo.Cats.Application.Features.Participants.Specifications;

public enum ParticipantListView
{
    [Description("Default")] Default,
    [Description("Identified")] Identified,
    [Description("Enrolling")] Enrolling,
    [Description("Submitted To Provider")] SubmittedToProvider,
    [Description("Submitted To QA")] SubmittedToQa,
    [Description("Any QA")] SubmittedToAny,
    [Description("Approved")] Approved,
    [Description("Dormant")] Dormant,
    [Description("Archived")] Archived,
    [Description("All")] All
}

public static class ParticipantListViewExtensions
{
    public static string GetDescription(this ParticipantListView e)
        => e switch
        {
            ParticipantListView.Default => "Default",
            ParticipantListView.Identified => "Identified",
            ParticipantListView.Enrolling => "Enrolling",
            ParticipantListView.SubmittedToProvider => "PQA",
            ParticipantListView.SubmittedToQa => "CFO QA",
            ParticipantListView.SubmittedToAny => "QA (any)",
            ParticipantListView.Approved => "Approved",
            ParticipantListView.Dormant => "Dormant",
            ParticipantListView.Archived => "Archived",
            ParticipantListView.All => "All",
            _ => throw new ArgumentOutOfRangeException("Unknown list view type")
        };
}