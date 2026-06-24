using AutoMapper;
using Cfo.Cats.Application.Common.Extensions;
using Cfo.Cats.Application.Common.Security;
using Cfo.Cats.Application.SecurityConstants;
using FluentValidation;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.EntityFrameworkCore;
using Toolbelt.Blazor.HotKeys2;

namespace Cfo.Cats.Server.UI.Components.Shared.Layout;

public partial class SearchDialog : ComponentBase, IDisposable
{
    [CascadingParameter] private IMudDialogInstance MudDialog { get; set; } = null!;

    [Inject] private NavigationManager Navigation { get; set; } = default!;
    [Inject] private HotKeys HotKeys { get; set; } = default!;

    [Parameter]
    public UserProfile? UserProfile { get; set; }

    private readonly Dictionary<string, string> _pages = new();
    private HotKeysContext? _hotKeysContext;
    private Dictionary<string, string> _pagesFiltered = new();
    private string _search = string.Empty;

    public void Dispose() => _hotKeysContext?.DisposeAsync();

    private void HandleTextFieldKeyDown(KeyboardEventArgs e)
    {
        if (e.Key == "Enter" && _pagesFiltered.Any())
        {
            // Assuming the first item should be navigated to on Enter
            var firstPage = _pagesFiltered.First().Value;
            NavigateToPage(firstPage);
        }
    }

    private void HandleListItemKeyDown(KeyboardEventArgs e, string url)
    {
        if (e.Key == "Enter")
        {
            NavigateToPage(url);
        }
    }

    private void NavigateToPage(string url) => Navigation.NavigateTo(url);

    protected override void OnInitialized()
    {
        _pages.Add("Home", "/");
        _pages.Add("Support Worker Dashboard", "/pages/dashboard/supportworker");
        _pages.Add("Participants", "/pages/workspace/participants");
        _pages.Add("New Enrolment", "/pages/candidates/search");
        _pages.Add("My Documents", "/pages/analytics/my-documents");

        if (UserProfile is not null)
        {
            if (UserProfile.AssignedRoles.Any())
            {
                _pages.Add("Reassign", "/pages/workspace/participants/reassign");
                _pages.Add("Transfers", "/pages/workspace/participants/transfers");
                _pages.Add("Moved Participants", "/pages/workspace/participants/movedparticipants");
                _pages.Add("Active PRI's", "/pages/workspace/participants/pre-release-inventory");
            }

            string[] allowed = [RoleNames.QAFinance, RoleNames.SMT, RoleNames.SystemSupport];
            if (UserProfile.AssignedRoles.Any(r => allowed.Contains(r)))
            {
                _pages.Add("Provider QA", "/pages/qa/enrolments/pqa");
            }

            allowed = [RoleNames.QAOfficer, RoleNames.QASupportManager, RoleNames.QAManager, RoleNames.SMT, RoleNames.SystemSupport];
            if (UserProfile.AssignedRoles.Any(r => allowed.Contains(r)))
            {
                _pages.Add("First Pass", "/pages/qa/enrolments/qa1");
            }

            allowed = [RoleNames.QASupportManager, RoleNames.QAManager, RoleNames.SMT, RoleNames.SystemSupport, RoleNames.QAOfficer];
            if (UserProfile.AssignedRoles.Any(r => allowed.Contains(r)))
            {
                _pages.Add("Second Pass", "/pages/qa/enrolments/qa2");
                _pages.Add("Quality Assurance", "/pages/workspace/servicedesk");
            }

            allowed = [RoleNames.QAManager, RoleNames.SMT, RoleNames.SystemSupport];
            if (UserProfile.AssignedRoles.Any(r => allowed.Contains(r)))
            {
                _pages.Add("Queue Management", "/pages/workspace/servicedesk/enrolments/queue");
            }

            allowed = [RoleNames.QAFinance, RoleNames.SMT, RoleNames.SystemSupport];
            if (UserProfile.AssignedRoles.Any(r => allowed.Contains(r)))
            {
                _pages.Add("Activities Provider QA", "/pages/qa/activities/pqa");
            }

            allowed = [RoleNames.QAOfficer, RoleNames.QASupportManager, RoleNames.QAManager, RoleNames.SMT, RoleNames.SystemSupport];
            if (UserProfile.AssignedRoles.Any(r => allowed.Contains(r)))
            {
                _pages.Add("Activities First Pass", "/pages/qa/activities/qa1");
            }

            allowed = [RoleNames.QASupportManager, RoleNames.QAManager, RoleNames.SMT, RoleNames.SystemSupport];
            if (UserProfile.AssignedRoles.Any(r => allowed.Contains(r)))
            {
                _pages.Add("Activities Second Pass", "/pages/qa/activities/qa2");
            }

            allowed = [RoleNames.Finance, RoleNames.QAFinance, RoleNames.SystemSupport, RoleNames.QASupportManager, RoleNames.QAManager, RoleNames.SMT];
            if (UserProfile.AssignedRoles.Any(r => allowed.Contains(r)))
            {
                _pages.Add("Payments", "/pages/finance/payments/");
                _pages.Add("Cumulatives", "/pages/analytics/cumulatives");
                _pages.Add("Outcome Quality Dip Sampling", "/pages/analytics/outcome-quality-dip-sampling");
            }

            allowed = [RoleNames.QAManager, RoleNames.SMT, RoleNames.SystemSupport];
            if (UserProfile.AssignedRoles.Any(r => allowed.Contains(r)))
            {
                _pages.Add("Activities Queue Management", "/pages/workspace/servicedesk/activities/queue");
            }

            allowed = [RoleNames.QASupportManager, RoleNames.QAManager, RoleNames.SMT, RoleNames.SystemSupport, RoleNames.QAOfficer];
            if (UserProfile.AssignedRoles.Any(r => allowed.Contains(r)))
            {
                _pages.Add("Service Desk", "/pages/workspace/servicedesk");
                _pages.Add("Activities Feedback", "/pages/workspace/servicedesk/activities/feedback");
                _pages.Add("Enrolments Feedback", "/pages/workspace/servicedesk/enrolments/feedback");
            }

            allowed = [RoleNames.SystemSupport, RoleNames.QAOfficer, RoleNames.QAManager, RoleNames.QASupportManager, RoleNames.SMT];

            if (UserProfile.AssignedRoles.Any(r => allowed.Contains(r)))
            {
                _pages.Add("Tenants", "/administration/tenants");
                _pages.Add("Users", "/identity/users");
                _pages.Add("Lookup Values", "/system/picklist");
                _pages.Add("Audit Trails", "/system/audittrails");
            }

            allowed = RoleNames.ManageLabels;

            if (UserProfile.AssignedRoles.Any(r => allowed.Contains(r)))
            {
                _pages.Add("Labels", "/pages/labels");
            }
        }

        _pagesFiltered = _pages;

        _hotKeysContext = HotKeys.CreateContext()
            .Add(Key.Escape, () => MudDialog.Close(), "Close command palette.");
    }

    private void SearchPages(string value)
    {
        _pagesFiltered = new Dictionary<string, string>();

        if (!string.IsNullOrWhiteSpace(value))
        {
            _pagesFiltered = _pages
                .Where(x => x.Key
                    .Contains(value, StringComparison.InvariantCultureIgnoreCase))
                .ToDictionary(x => x.Key, x => x.Value);
        }
        else
        {
            _pagesFiltered = _pages;
        }
    }
}