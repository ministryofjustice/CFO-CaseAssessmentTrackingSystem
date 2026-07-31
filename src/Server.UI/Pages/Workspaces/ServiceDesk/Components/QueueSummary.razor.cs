using Cfo.Cats.Application.Common.Security;
using Cfo.Cats.Application.SecurityConstants;
using Cfo.Cats.Application.Features.Activities.Commands;
using Cfo.Cats.Application.Features.QualityAssurance.Commands;
using Cfo.Cats.Application.Features.QualityAssurance.Queries;
using Cfo.Cats.Application.Features.QualityAssurance.DTOs;

namespace Cfo.Cats.Server.UI.Pages.Workspaces.ServiceDesk.Components;

public partial class QueueSummary
{
    [Parameter]
    public string? TenantId { get; set; }

    [Parameter]
    public bool ShowEnrolments { get; set; } = true;

    [Parameter]
    public bool ShowActivities { get; set; } = true;

    [Parameter]
    public EventCallback<string> OnViewQueueNavigate { get; set; }

    private bool _grabbing;

    private bool CanViewFirstPass => HasAnyRole(RoleNames.QAOfficer, RoleNames.QAManager, RoleNames.QASupportManager, RoleNames.SMT, RoleNames.SystemSupport);
    private bool CanViewSecondPass => HasAnyRole(RoleNames.QAManager, RoleNames.QASupportManager, RoleNames.SMT, RoleNames.SystemSupport);
    private bool CanViewEscalation => HasAnyRole(RoleNames.QAManager, RoleNames.QASupportManager, RoleNames.SMT, RoleNames.SystemSupport);

    protected override IQuery<Result<ServiceDeskQueueSummaryDto>> CreateQuery() =>
        new GetServiceDeskQueueSummary.Query
        {
            CurrentUser = CurrentUser,
            ShowEnrolments = ShowEnrolments,
            ShowActivities = ShowActivities
        };
    private async Task GrabEnrolmentQa1()
    {
        if (_grabbing)
        {
            return;
        }

        try
        {
            _grabbing = true;

            var result = await Service.Send(new GrabQa1Entry.Command
            {
                CurrentUser = CurrentUser
            });

            if (result.Succeeded)
            {
                Snackbar.Add("Assigned next Enrolment QA1 case.", Severity.Success);
                Navigation.NavigateTo($"/pages/workspace/servicedesk/enrolments/qa1?queueEntryId={result.Data!.Id}");
                return;
            }

            Snackbar.Add(result.ErrorMessage, Severity.Info);
        }
        finally
        {
            _grabbing = false;
        }
    }

    private async Task GrabEnrolmentQa2()
    {
        if (_grabbing)
        {
            return;
        }

        try
        {
            _grabbing = true;

            var result = await Service.Send(new GrabQa2Entry.Command
            {
                CurrentUser = CurrentUser
            });

            if (result.Succeeded)
            {
                Snackbar.Add("Assigned next Enrolment QA2 case.", Severity.Success);
                Navigation.NavigateTo($"/pages/workspace/servicedesk/enrolments/qa2?queueEntryId={result.Data!.Id}");
                return;
            }

            Snackbar.Add(result.ErrorMessage, Severity.Info);
        }
        finally
        {
            _grabbing = false;
        }
    }

    private async Task GrabActivityQa1()
    {
        if (_grabbing)
        {
            return;
        }

        try
        {
            _grabbing = true;

            var result = await Service.Send(new GrabActivityQa1Entry.Command
            {
                CurrentUser = CurrentUser
            });

            if (result.Succeeded)
            {
                Snackbar.Add("Assigned next Activity QA1 case.", Severity.Success);
                Navigation.NavigateTo($"/pages/workspace/servicedesk/activities/qa1?queueEntryId={result.Data!.Id}");
                return;
            }

            Snackbar.Add(result.ErrorMessage, Severity.Info);
        }
        finally
        {
            _grabbing = false;
        }
    }

    private async Task GrabActivityQa2()
    {
        if (_grabbing)
        {
            return;
        }

        try
        {
            _grabbing = true;

            var result = await Service.Send(new GrabActivityQa2Entry.Command
            {
                CurrentUser = CurrentUser
            });

            if (result.Succeeded)
            {
                Snackbar.Add("Assigned next Activity QA2 case.", Severity.Success);
                Navigation.NavigateTo($"/pages/workspace/servicedesk/activities/qa2?queueEntryId={result.Data!.Id}");
                return;
            }

            Snackbar.Add(result.ErrorMessage, Severity.Info);
        }
        finally
        {
            _grabbing = false;
        }
    }

    private bool HasAnyRole(params string[] roles) =>
        CurrentUser.AssignedRoles.Any(userRole => roles.Contains(userRole));
}