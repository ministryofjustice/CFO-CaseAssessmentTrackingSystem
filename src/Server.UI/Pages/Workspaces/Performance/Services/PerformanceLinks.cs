namespace Cfo.Cats.Server.UI.Pages.Workspaces.Performance.Services;

public static class PerformanceLinks
{
    public static PerformanceLink Home => new("Home", "/pages/workspace/performance");

    public static PerformanceLink OutcomeQualityDipSamples = new("Outcome Quality", $"{Home.Url}/outcome-quality-dip-sampling");

    public static PerformanceLink OutcomeQualityDipSample(Guid sampleId) => new("Outcome Quality", $"{OutcomeQualityDipSamples.Url}/{sampleId}");

    public static PerformanceLink OutcomeQualityDipSampleParticipant(Guid sampleId, string participantId) => new(participantId, $"{OutcomeQualityDipSamples.Url}/{sampleId}/{participantId}");

    public static PerformanceLink ArchivedCaseBehaviour => new ("Archived Case Behaviour" ,$"{Home.Url}/archived-case-behaviour");

    public record PerformanceLink(string Title, string Url);
}
