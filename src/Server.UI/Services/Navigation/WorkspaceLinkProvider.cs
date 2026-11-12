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

public sealed record WorkspaceLink(Workspace Workspace, BreadcrumbLinkModel Link);

/// <summary>
/// Single source of truth for which workspace sub pages a user may see. Used by the
/// workspace landing pages and by global search so both apply the same rules.
/// </summary>
public interface IWorkspaceLinkProvider
{
    Task<IReadOnlyList<BreadcrumbLinkModel>> GetLinksAsync(Workspace workspace, ClaimsPrincipal user);

    Task<IReadOnlyList<WorkspaceLink>> GetAllLinksAsync(ClaimsPrincipal user);
}

public sealed class WorkspaceLinkProvider(IAuthorizationService authorizationService, IConfiguration configuration)
    : IWorkspaceLinkProvider
{
    private sealed record Rule(Workspace Workspace, BreadcrumbLinkModel Link, Func<Evaluator, Task<bool>> IsVisible);

    private sealed class Evaluator(ClaimsPrincipal user, IAuthorizationService authorizationService, IConfiguration configuration)
    {
        public async Task<bool> Policy(string policy)
            => (await authorizationService.AuthorizeAsync(user, policy)).Succeeded;

        public bool InAnyRole(params string[] roles) => roles.Any(user.IsInRole);

        public bool Feature(string section) => configuration.IsFeatureEnabled(section);
    }

    private static readonly Task<bool> Always = Task.FromResult(true);

    private static Rule Open(Workspace w, BreadcrumbLinkModel link) => new(w, link, _ => Always);
    private static Rule ByPolicy(Workspace w, BreadcrumbLinkModel link, string policy) => new(w, link, e => e.Policy(policy));
    private static Rule When(Workspace w, BreadcrumbLinkModel link, Func<Evaluator, Task<bool>> predicate) => new(w, link, predicate);

    private static readonly IReadOnlyList<Rule> Rules =
    [
        // Participants
        Open(Workspace.Participants, ParticipantLinks.All),
        Open(Workspace.Participants, ParticipantLinks.AllActivities),
        Open(Workspace.Participants, ParticipantLinks.MovedParticipants),
        Open(Workspace.Participants, ParticipantLinks.TasksDue),
        Open(Workspace.Participants, ParticipantLinks.AllPris),
        ByPolicy(Workspace.Participants, ParticipantLinks.IncomingTransfers, SecurityPolicies.Transfers),
        ByPolicy(Workspace.Participants, ParticipantLinks.OutgoingTransfers, SecurityPolicies.Transfers),

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
        When(Workspace.DeliveryManagement, DeliveryManagementLinks.Cumulatives, e => Task.FromResult(e.InAnyRole(RoleNames.SystemSupport, RoleNames.Finance))),
        ByPolicy(Workspace.DeliveryManagement, DeliveryManagementLinks.EnrolmentsPqa, SecurityPolicies.Pqa),
        ByPolicy(Workspace.DeliveryManagement, DeliveryManagementLinks.ActivitiesPqa, SecurityPolicies.Pqa),
        ByPolicy(Workspace.DeliveryManagement, DeliveryManagementLinks.Feedback, SecurityPolicies.DeliveryFeedbackView),
        When(Workspace.DeliveryManagement, DeliveryManagementLinks.Payments, e => Task.FromResult(e.InAnyRole(RoleNames.SystemSupport, RoleNames.Finance))),
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
        Open(Workspace.MyArea, MyAreaLinks.PasswordReset),
        Open(Workspace.MyArea, MyAreaLinks.MyDocuments),
        Open(Workspace.MyArea, MyAreaLinks.Notifications),
    ];

    private static async Task<bool> CanViewQueues(Evaluator e)
        => e.InAnyRole(RoleNames.QASupportManager, RoleNames.QAManager, RoleNames.SMT, RoleNames.SystemSupport)
           && await e.Policy(SecurityPolicies.Qa1);

    private static async Task<bool> HmppsApi(Evaluator e)
        => e.Feature(FeatureSections.HmppsExternalApi) && await e.Policy(SecurityPolicies.SystemSupportFunctions);

    public async Task<IReadOnlyList<BreadcrumbLinkModel>> GetLinksAsync(Workspace workspace, ClaimsPrincipal user)
        => (await EvaluateAsync(user, workspace)).Select(r => r.Link).ToArray();

    public async Task<IReadOnlyList<WorkspaceLink>> GetAllLinksAsync(ClaimsPrincipal user)
        => (await EvaluateAsync(user, null)).Select(r => new WorkspaceLink(r.Workspace, r.Link)).ToArray();

    private async Task<List<Rule>> EvaluateAsync(ClaimsPrincipal user, Workspace? workspace)
    {
        var evaluator = new Evaluator(user, authorizationService, configuration);
        var visible = new List<Rule>();

        foreach (var rule in Rules.Where(r => workspace is null || r.Workspace == workspace))
        {
            if (await rule.IsVisible(evaluator))
            {
                visible.Add(rule);
            }
        }

        return visible;
    }
}
