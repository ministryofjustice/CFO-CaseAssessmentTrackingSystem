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
    private int _enrolmentQa1Count;
    private int _enrolmentQa2Count;
    private int _enrolmentEscalationCount;
    private int _activityQa1Count;
    private int _activityQa2Count;
    private int _activityEscalationCount;
    private string? _lastTenantId;
    private string? _lastUserId;
    private bool _lastShowEnrolments = true;
    private bool _lastShowActivities = true;
    private bool _hasLoadedOnce;

    private bool CanViewFirstPass => HasAnyRole(RoleNames.QAOfficer, RoleNames.QAManager, RoleNames.QASupportManager, RoleNames.SMT, RoleNames.SystemSupport);
    private bool CanViewSecondPass => HasAnyRole(RoleNames.QAManager, RoleNames.QASupportManager, RoleNames.SMT, RoleNames.SystemSupport);
    private bool CanViewEscalation => HasAnyRole(RoleNames.QAManager, RoleNames.QASupportManager, RoleNames.SMT, RoleNames.SystemSupport);

    protected override async Task OnParametersSetAsync()
    {
        if (CurrentUser is null)
        {
            return;
        }

        var hasChanged = _lastTenantId != TenantId
            || _lastUserId != CurrentUser.UserId
            || _lastShowEnrolments != ShowEnrolments
            || _lastShowActivities != ShowActivities;

        _lastTenantId = TenantId;
        _lastUserId = CurrentUser.UserId;
        _lastShowEnrolments = ShowEnrolments;
        _lastShowActivities = ShowActivities;

        if (_hasLoadedOnce && hasChanged)
        {
            await RefreshAsync();
        }
    }

    protected override IQuery<Result<ServiceDeskQueueSummaryDto>> CreateQuery() =>
        new GetServiceDeskQueueSummary.Query
        {
            CurrentUser = CurrentUser,
            ShowEnrolments = ShowEnrolments,
            ShowActivities = ShowActivities
        };

    protected override void OnDataLoaded(ServiceDeskQueueSummaryDto data)
    {
        _enrolmentQa1Count = data.EnrolmentQa1Count;
        _enrolmentQa2Count = data.EnrolmentQa2Count;
        _enrolmentEscalationCount = data.EnrolmentEscalationCount;
        _activityQa1Count = data.ActivityQa1Count;
        _activityQa2Count = data.ActivityQa2Count;
        _activityEscalationCount = data.ActivityEscalationCount;
        _hasLoadedOnce = true;
        base.OnDataLoaded(data);
    }

    private static Color GetCountColor(int _) => Color.Default;

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
                Navigation.NavigateTo($"/pages/qa/enrolments/qa1/?queueEntryId={result.Data!.Id}");
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
                Navigation.NavigateTo($"/pages/qa/enrolments/qa2/?queueEntryId={result.Data!.Id}");
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
                Navigation.NavigateTo($"/pages/qa/activities/qa1/?queueEntryId={result.Data!.Id}");
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
                Navigation.NavigateTo($"/pages/qa/activities/qa2/?queueEntryId={result.Data!.Id}");
                return;
            }

            Snackbar.Add(result.ErrorMessage, Severity.Info);
        }
        finally
        {
            _grabbing = false;
        }
    }

    private Task ViewQueue(string target) =>
        OnViewQueueNavigate.HasDelegate
            ? OnViewQueueNavigate.InvokeAsync(target)
            : Task.CompletedTask;

    private bool HasAnyRole(params string[] roles) =>
        CurrentUser.AssignedRoles.Any(userRole => roles.Contains(userRole));
}