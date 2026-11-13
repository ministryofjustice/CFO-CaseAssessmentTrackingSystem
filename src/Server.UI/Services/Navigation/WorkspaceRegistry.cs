using System.Security.Claims;
using Cfo.Cats.Application.Common.Configuration;
using Cfo.Cats.Application.SecurityConstants;
using Cfo.Cats.Server.UI.Models.Breadcrumb;
using Cfo.Cats.Server.UI.Pages.Workspaces.Administration.Services;
using Cfo.Cats.Server.UI.Pages.Workspaces.DeliveryManagement.Services;
using Cfo.Cats.Server.UI.Pages.Workspaces.MyArea.Services;
using Cfo.Cats.Server.UI.Pages.Workspaces.Participants.Services;
using Cfo.Cats.Server.UI.Pages.Workspaces.Performance.Services;
using Cfo.Cats.Server.UI.Pages.Workspaces.ServiceDesk.Services;
using Microsoft.AspNetCore.Authorization;

namespace Cfo.Cats.Server.UI.Services.Navigation;

public enum Workspace
{
    Participants,
    DeliveryManagement,
    ServiceDesk,
    Performance,
    Administration,
    MyArea
}

public sealed class WorkspaceLinkEvaluator(ClaimsPrincipal user, IAuthorizationService authorizationService, IConfiguration configuration)
{
    public async Task<bool> Policy(string policy)
        => (await authorizationService.AuthorizeAsync(user, policy)).Succeeded;

    public bool InAnyRole(params string[] roles) => roles.Any(user.IsInRole);

    public bool Feature(string section) => configuration.IsFeatureEnabled(section);
}

public sealed record WorkspaceEntry(
    Workspace Workspace,
    BreadcrumbLinkModel Link,
    Func<WorkspaceLinkEvaluator, Task<bool>> IsVisible,
    BreadcrumbLinkModel? Parent = null,
    string? Policy = null,
    string? Roles = null);

/// <summary>
/// The breadcrumb items that lead to a page, plus any part of the path the registry does not know
/// about (for example a participant or sample id) so the caller can append dynamic crumbs.
/// </summary>
public sealed record WorkspaceTrail(IReadOnlyList<BreadcrumbLinkModel> Items, string[] RemainingSegments);

/// <summary>
/// Single source of truth for workspace sub pages: where they live, their parent in the
/// breadcrumb trail and who can see them.
/// </summary>
public static class WorkspaceRegistry
{
    private static readonly Task<bool> Always = Task.FromResult(true);

    private static WorkspaceEntry Open(Workspace w, BreadcrumbLinkModel link, BreadcrumbLinkModel? parent = null)
        => new(w, link, _ => Always, parent);

    private static WorkspaceEntry ByPolicy(Workspace w, BreadcrumbLinkModel link, string policy)
        => new(w, link, e => e.Policy(policy), Policy: policy);

    private static WorkspaceEntry ByRoles(Workspace w, BreadcrumbLinkModel link, params string[] roles)
        => new(w, link, e => Task.FromResult(e.InAnyRole(roles)), Roles: string.Join(", ", roles));

    private static WorkspaceEntry When(Workspace w, BreadcrumbLinkModel link, Func<WorkspaceLinkEvaluator, Task<bool>> predicate)
        => new(w, link, predicate);

    public static BreadcrumbLinkModel HomeOf(Workspace workspace) => workspace switch
    {
        Workspace.Participants => ParticipantLinks.Home,
        Workspace.DeliveryManagement => DeliveryManagementLinks.Home,
        Workspace.ServiceDesk => ServiceDeskLinks.Home,
        Workspace.Performance => PerformanceLinks.Home,
        Workspace.Administration => AdministrationLinks.Home,
        Workspace.MyArea => MyAreaLinks.Home,
        _ => throw new ArgumentOutOfRangeException(nameof(workspace), workspace, null)
    };

    public static readonly IReadOnlyList<WorkspaceEntry> Entries =
    [
        // Participants
        Open(Workspace.Participants, ParticipantLinks.All),
        Open(Workspace.Participants, ParticipantLinks.AllActivities),
        Open(Workspace.Participants, ParticipantLinks.MovedParticipants),
        Open(Workspace.Participants, ParticipantLinks.TasksDue),
        Open(Workspace.Participants, ParticipantLinks.AllPris),
        ByPolicy(Workspace.Participants, ParticipantLinks.IncomingTransfers, SecurityPolicies.UserHasAdditionalRoles),
        ByPolicy(Workspace.Participants, ParticipantLinks.OutgoingTransfers, SecurityPolicies.UserHasAdditionalRoles),

        // Delivery management
        Open(Workspace.DeliveryManagement, DeliveryManagementLinks.Performance),
        Open(Workspace.DeliveryManagement, DeliveryManagementLinks.LocationBreakdown),
        Open(Workspace.DeliveryManagement, DeliveryManagementLinks.LatestEngagements),
        Open(Workspace.DeliveryManagement, DeliveryManagementLinks.EngagementsByLocation),
        Open(Workspace.DeliveryManagement, DeliveryManagementLinks.PathwayPlanReviews),
        Open(Workspace.DeliveryManagement, DeliveryManagementLinks.Initiatives),
        Open(Workspace.DeliveryManagement, DeliveryManagementLinks.RecentApprovedActivities),
        Open(Workspace.DeliveryManagement, DeliveryManagementLinks.RiskDue),
        Open(Workspace.DeliveryManagement, DeliveryManagementLinks.ActivitiesInQaPots),
        ByPolicy(Workspace.DeliveryManagement, DeliveryManagementLinks.CaseWorkload, SecurityPolicies.UserHasAdditionalRoles),
        ByPolicy(Workspace.DeliveryManagement, DeliveryManagementLinks.UnassignedCases, SecurityPolicies.UserHasAdditionalRoles),
        ByPolicy(Workspace.DeliveryManagement, DeliveryManagementLinks.ArchivedCaseBehaviour, SecurityPolicies.UserHasAdditionalRoles),
        ByRoles(Workspace.DeliveryManagement, DeliveryManagementLinks.Cumulatives, RoleNames.SystemSupport, RoleNames.Finance),
        ByPolicy(Workspace.DeliveryManagement, DeliveryManagementLinks.EnrolmentsPqa, SecurityPolicies.Pqa),
        ByPolicy(Workspace.DeliveryManagement, DeliveryManagementLinks.ActivitiesPqa, SecurityPolicies.Pqa),
        ByPolicy(Workspace.DeliveryManagement, DeliveryManagementLinks.Feedback, SecurityPolicies.DeliveryFeedbackView),
        ByRoles(Workspace.DeliveryManagement, DeliveryManagementLinks.Payments, RoleNames.SystemSupport, RoleNames.Finance),
        Open(Workspace.DeliveryManagement, DeliveryManagementLinks.QaEnrolmentResults),

        // Service desk
        When(Workspace.ServiceDesk, ServiceDeskLinks.ActivitiesQueue, CanViewQueues),
        ByPolicy(Workspace.ServiceDesk, ServiceDeskLinks.ActivitiesFeedback, SecurityPolicies.Qa1),
        When(Workspace.ServiceDesk, ServiceDeskLinks.EnrolmentsQueue, CanViewQueues),
        ByPolicy(Workspace.ServiceDesk, ServiceDeskLinks.EnrolmentsFeedback, SecurityPolicies.Qa1),
        ByPolicy(Workspace.ServiceDesk, ServiceDeskLinks.QaOfficerStats, SecurityPolicies.ServiceDeskManagement),
        ByPolicy(Workspace.ServiceDesk, ServiceDeskLinks.SyncInfo, SecurityPolicies.ServiceDeskSyncInformation),
        ByPolicy(Workspace.ServiceDesk, ServiceDeskLinks.QaPots, SecurityPolicies.Internal),

        // Performance
        ByPolicy(Workspace.Performance, PerformanceLinks.OutcomeQualityDipSamples, SecurityPolicies.OutcomeQualityDipChecks),
        ByPolicy(Workspace.Performance, PerformanceLinks.Initiatives, SecurityPolicies.Initiatives),

        // Administration
        ByPolicy(Workspace.Administration, AdministrationLinks.Labels, SecurityPolicies.SeniorInternal),
        ByPolicy(Workspace.Administration, AdministrationLinks.HelpLinks, SecurityPolicies.SeniorInternal),
        ByPolicy(Workspace.Administration, AdministrationLinks.ContractTargets, SecurityPolicies.SeniorInternal),
        ByPolicy(Workspace.Administration, AdministrationLinks.AuditTrails, SecurityPolicies.ServiceDeskManagement),
        ByPolicy(Workspace.Administration, AdministrationLinks.UsageTelemetry, SecurityPolicies.ServiceDeskManagement),
        ByPolicy(Workspace.Administration, AdministrationLinks.Outbox, SecurityPolicies.ServiceDeskManagement),
        ByPolicy(Workspace.Administration, AdministrationLinks.PickList, SecurityPolicies.ServiceDeskManagement),
        When(Workspace.Administration, AdministrationLinks.IntegrationEvents,
            async e => e.Feature(FeatureSections.IntegrationEventPublishing) && await e.Policy(SecurityPolicies.ServiceDeskManagement)),
        ByPolicy(Workspace.Administration, AdministrationLinks.Tenants, SecurityPolicies.SystemFunctionsRead),
        ByPolicy(Workspace.Administration, AdministrationLinks.Users, SecurityPolicies.SystemFunctionsRead),
        ByPolicy(Workspace.Administration, AdministrationLinks.UserAudit, SecurityPolicies.SystemFunctionsRead),
        ByPolicy(Workspace.Administration, AdministrationLinks.LoginMonitoring, SecurityPolicies.SystemFunctionsRead),
        ByPolicy(Workspace.Administration, AdministrationLinks.Jobs, SecurityPolicies.SystemSupportFunctions),
        ByPolicy(Workspace.Administration, AdministrationLinks.CacheManagement, SecurityPolicies.SystemSupportFunctions),
        When(Workspace.Administration, AdministrationLinks.HmppsRisk, HmppsApi),
        When(Workspace.Administration, AdministrationLinks.HmppsPerson, HmppsApi),
        When(Workspace.Administration, AdministrationLinks.HmppsTier, HmppsApi),

        // My area
        Open(Workspace.MyArea, MyAreaLinks.EditProfile),
        Open(Workspace.MyArea, MyAreaLinks.PasswordReset, parent: MyAreaLinks.EditProfile),
        Open(Workspace.MyArea, MyAreaLinks.MyDocuments),
        Open(Workspace.MyArea, MyAreaLinks.Notifications),
    ];

    private static async Task<bool> CanViewQueues(WorkspaceLinkEvaluator e)
        => e.InAnyRole(RoleNames.QASupportManager, RoleNames.QAManager, RoleNames.SMT, RoleNames.SystemSupport)
           && await e.Policy(SecurityPolicies.Qa1);

    private static async Task<bool> HmppsApi(WorkspaceLinkEvaluator e)
        => e.Feature(FeatureSections.HmppsExternalApi) && await e.Policy(SecurityPolicies.SystemSupportFunctions);

    /// <summary>
    /// Builds the breadcrumb trail for a path within a workspace by matching the longest registered page.
    /// </summary>
    public static WorkspaceTrail TrailFor(Workspace workspace, string absolutePath)
    {
        var home = HomeOf(workspace);
        var path = absolutePath.TrimEnd('/');

        var match = Entries
            .Where(e => e.Workspace == workspace)
            .Where(e => path.Equals(e.Link.Href, StringComparison.OrdinalIgnoreCase)
                        || path.StartsWith(e.Link.Href + "/", StringComparison.OrdinalIgnoreCase))
            .OrderByDescending(e => e.Link.Href.Length)
            .FirstOrDefault();

        if (match is null)
        {
            var homeRemainder = path.Length > home.Href.Length ? path[home.Href.Length..] : string.Empty;
            return new WorkspaceTrail([home], Split(homeRemainder));
        }

        List<BreadcrumbLinkModel> items = [home];
        if (match.Parent is not null)
        {
            items.Add(match.Parent);
        }
        items.Add(match.Link);

        return new WorkspaceTrail(items, Split(path[match.Link.Href.Length..]));
    }

    private static string[] Split(string remainder) => remainder.Split('/', StringSplitOptions.RemoveEmptyEntries);
}
