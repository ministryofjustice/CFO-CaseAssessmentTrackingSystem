using Cfo.Cats.Application.Common.Security;
using Cfo.Cats.Application.SecurityConstants;
using Cfo.Cats.Application.Features.Activities.Commands;
using Cfo.Cats.Application.Features.Activities.Queries;
using Cfo.Cats.Application.Features.QualityAssurance.Commands;
using Cfo.Cats.Application.Features.QualityAssurance.Queries;

namespace Cfo.Cats.Server.UI.Pages.Workspaces.ServiceDesk.Pages.QualityAssurance.Components;

public partial class QaStageCountsCards
{
    [Parameter, EditorRequired]
    public UserProfile CurrentUser { get; set; } = null!;

    [Parameter]
    public string? TenantId { get; set; }

    [Parameter]
    public bool ShowEnrolments { get; set; } = true;

    [Parameter]
    public bool ShowActivities { get; set; } = true;

    [Parameter]
    public EventCallback<string> OnViewQueueNavigate { get; set; }

    private bool _loading;
    private bool _grabbing;
    private int _enrolmentQa1Count;
    private int _enrolmentQa2Count;
    private int _enrolmentEscalationCount;
    private int _activityQa1Count;
    private int _activityQa2Count;
    private int _activityEscalationCount;
    private string? _lastTenantId;
    private string? _lastUserId;

    private bool CanViewFirstPass => HasAnyRole(RoleNames.QAOfficer, RoleNames.QAManager, RoleNames.QASupportManager, RoleNames.SMT, RoleNames.SystemSupport);
    private bool CanViewSecondPass => HasAnyRole(RoleNames.QAManager, RoleNames.QASupportManager, RoleNames.SMT, RoleNames.SystemSupport);
    private bool CanViewEscalation => HasAnyRole(RoleNames.QAManager, RoleNames.QASupportManager, RoleNames.SMT, RoleNames.SystemSupport);

    protected override async Task OnParametersSetAsync()
    {
        if (CurrentUser is null)
        {
            return;
        }

        if (_lastTenantId == TenantId && _lastUserId == CurrentUser.UserId)
        {
            return;
        }

        await LoadCounts();
        _lastTenantId = TenantId;
        _lastUserId = CurrentUser.UserId;
    }

    private async Task LoadCounts()
    {
        try
        {
            _loading = true;

            if (ShowEnrolments)
            {
                if (CanViewFirstPass)
                {
                    var qa1Result = await GetNewMediator().Send(new Qa1WithPagination.Query
                    {
                        CurrentUser = CurrentUser,
                        TenantId = TenantId,
                        PageNumber = 1,
                        PageSize = 1,
                        OrderBy = "Created",
                        SortDirection = "Descending"
                    });

                    _enrolmentQa1Count = qa1Result is { Succeeded: true, Data: not null }
                        ? qa1Result.Data.TotalItems
                        : 0;
                }

                if (CanViewSecondPass)
                {
                    var qa2Result = await GetNewMediator().Send(new Qa2WithPagination.Query
                    {
                        CurrentUser = CurrentUser,
                        TenantId = TenantId,
                        PageNumber = 1,
                        PageSize = 1,
                        OrderBy = "Created",
                        SortDirection = "Descending"
                    });

                    _enrolmentQa2Count = qa2Result is { Succeeded: true, Data: not null }
                        ? qa2Result.Data.TotalItems
                        : 0;
                }

                if (CanViewEscalation)
                {
                    var escalationResult = await GetNewMediator().Send(new QaEscalationWithPagination.Query
                    {
                        CurrentUser = CurrentUser,
                        TenantId = TenantId,
                        PageNumber = 1,
                        PageSize = 1,
                        OrderBy = "Created",
                        SortDirection = "Descending"
                    });

                    _enrolmentEscalationCount = escalationResult is { Succeeded: true, Data: not null }
                        ? escalationResult.Data.TotalItems
                        : 0;
                }
            }

            if (ShowActivities)
            {
                if (CanViewFirstPass)
                {
                    var activityQa1Result = await GetNewMediator().Send(new ActivityQa1WithPagination.Query
                    {
                        CurrentUser = CurrentUser,
                        TenantId = TenantId,
                        PageNumber = 1,
                        PageSize = 1,
                        OrderBy = "Created",
                        SortDirection = "Descending"
                    });

                    _activityQa1Count = activityQa1Result is { Succeeded: true, Data: not null }
                        ? activityQa1Result.Data.TotalItems
                        : 0;
                }

                if (CanViewSecondPass)
                {
                    var activityQa2Result = await GetNewMediator().Send(new ActivityQa2WithPagination.Query
                    {
                        CurrentUser = CurrentUser,
                        TenantId = TenantId,
                        PageNumber = 1,
                        PageSize = 1,
                        OrderBy = "Created",
                        SortDirection = "Descending"
                    });

                    _activityQa2Count = activityQa2Result is { Succeeded: true, Data: not null }
                        ? activityQa2Result.Data.TotalItems
                        : 0;
                }

                if (CanViewEscalation)
                {
                    var activityEscalationResult = await GetNewMediator().Send(new ActivityQaEscalationWithPagination.Query
                    {
                        CurrentUser = CurrentUser,
                        TenantId = TenantId,
                        PageNumber = 1,
                        PageSize = 1,
                        OrderBy = "Created",
                        SortDirection = "Descending"
                    });

                    _activityEscalationCount = activityEscalationResult is { Succeeded: true, Data: not null }
                        ? activityEscalationResult.Data.TotalItems
                        : 0;
                }
            }
        }
        finally
        {
            _loading = false;
        }
    }

    private static Color GetCountColor(int count) => count == 0 ? Color.Success : Color.Warning;

    private async Task GrabEnrolmentQa1()
    {
        if (_grabbing)
        {
            return;
        }

        try
        {
            _grabbing = true;

            var result = await GetNewMediator().Send(new GrabQa1Entry.Command
            {
                CurrentUser = CurrentUser
            });

            if (result.Succeeded)
            {
                Snackbar.Add("Assigned next Enrolment QA1 case.", Severity.Success);
                Navigation.NavigateTo("/pages/qa/enrolments/qa1/?AutoGrab=true");
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

            var result = await GetNewMediator().Send(new GrabQa2Entry.Command
            {
                CurrentUser = CurrentUser
            });

            if (result.Succeeded)
            {
                Snackbar.Add("Assigned next Enrolment QA2 case.", Severity.Success);
                Navigation.NavigateTo("/pages/qa/enrolments/qa2/?AutoGrab=true");
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

            var result = await GetNewMediator().Send(new GrabActivityQa1Entry.Command
            {
                CurrentUser = CurrentUser
            });

            if (result.Succeeded)
            {
                Snackbar.Add("Assigned next Activity QA1 case.", Severity.Success);
                Navigation.NavigateTo("/pages/qa/activities/qa1/?AutoGrab=true");
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

            var result = await GetNewMediator().Send(new GrabActivityQa2Entry.Command
            {
                CurrentUser = CurrentUser
            });

            if (result.Succeeded)
            {
                Snackbar.Add("Assigned next Activity QA2 case.", Severity.Success);
                Navigation.NavigateTo("/pages/qa/activities/qa2/?AutoGrab=true");
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
