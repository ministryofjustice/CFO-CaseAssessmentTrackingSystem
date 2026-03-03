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

