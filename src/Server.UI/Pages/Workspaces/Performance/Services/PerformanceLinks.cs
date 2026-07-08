using Cfo.Cats.Server.UI.Models.Breadcrumb;

namespace Cfo.Cats.Server.UI.Pages.Workspaces.Performance.Services;

public static class PerformanceLinks
{
    public static BreadcrumbLinkModel Home => new("Performance Management", "Home page for performance" ,"/pages/workspace/performance");

    public static BreadcrumbLinkModel OutcomeQualityDipSamples = new("Outcome Quality", "Review and assess outcome quality dip samples" , $"{Home.Href}/outcome-quality-dip-sampling");

    public static BreadcrumbLinkModel OutcomeQualityDipSample(Guid sampleId) => new("Outcome Quality", "Link for specific sample", $"{OutcomeQualityDipSamples.Href}/{sampleId}");

    public static BreadcrumbLinkModel OutcomeQualityDipSampleParticipant(Guid sampleId, string participantId) => new(participantId, "Link for specific participant" , $"{OutcomeQualityDipSamples.Href}/{sampleId}/{participantId}");

    public static BreadcrumbLinkModel ArchivedCaseBehaviour => new ("Archived Case Behaviour", "View participants moving into and out of archiving" , $"{Home.Href}/archived-case-behaviour");
}
