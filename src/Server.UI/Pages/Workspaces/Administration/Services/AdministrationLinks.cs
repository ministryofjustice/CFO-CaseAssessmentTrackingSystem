using Cfo.Cats.Server.UI.Models.Breadcrumb;

namespace Cfo.Cats.Server.UI.Pages.Workspaces.Administration.Services;

public static class AdministrationLinks
{
    public static BreadcrumbLinkModel Home => new("Administration", "", "/pages/workspace/Administration");
    public static BreadcrumbLinkModel Jobs = new("Jobs", "Job scheduler", $"{Home.Href}/Jobs");
    public static BreadcrumbLinkModel CacheManagement = new("Cache Management", "Cache Management", $"{Home.Href}/CacheManagement");
    // public static BreadcrumbLinkModel Participant(string id) => new(id, "Participant by id" , $"{Home.Href}/{id}");
    //
    // public static BreadcrumbLinkModel AllPris = new ("Active PRI", "Displays a list of active PRIs" , $"{Home.Href}/pre-release-inventory");
    //
    // public static BreadcrumbLinkModel MovedParticipants = new ("Moved", "Participants you are losing access to as they have moved" , $"{Home.Href}/moved");
    //
    // public static BreadcrumbLinkModel Transfers = new ("Transfers", "Manage incoming and view outgoing transfers", $"{Home.Href}/transfers");
    //
    // public static BreadcrumbLinkModel AllActivities = new ("Activities", "Access all activities" ,$"{Home.Href}/activities");
    //
    // public static BreadcrumbLinkModel ParticipantByActivity(string id) => new(id, "Participant links" ,$"{AllActivities}/{id}");

}

