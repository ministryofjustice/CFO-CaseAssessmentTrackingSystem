using Cfo.Cats.Application.Common.Security;
using Cfo.Cats.Application.SecurityConstants;
using Cfo.Cats.Application.Features.Activities.Commands;
using Cfo.Cats.Application.Features.Activities.Queries;
using Cfo.Cats.Application.Features.QualityAssurance.Commands;
using Cfo.Cats.Application.Features.QualityAssurance.Queries;

namespace Cfo.Cats.Server.UI.Pages.Workspaces.ServiceDesk.Components;

public partial class QueueSummary
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

    private bool _loading = true;
    private bool _grabbing;
    private int _enrolmentQa1Count;
    private int _enrolmentQa2Count;
    private int _enrolmentEscalationCount;
    private int _activityQa1Count;
    private int _activityQa2Count;
    private int _activityEscalationCount;
    private string? _errorMessage;
    private bool _isLoadingCounts;
    private bool _reloadRequested;
    private string? _lastTenantId;
    private string? _lastUserId;

    private bool CanViewFirstPass => HasAnyRole(RoleNames.QAOfficer, RoleNames.QAManager, RoleNames.QASupportManager, RoleNames.SMT, RoleNames.SystemSupport);
    private bool CanViewSecondPass => HasAnyRole(RoleNames.QAManager, RoleNames.QASupportManager, RoleNames.SMT, RoleNames.SystemSupport);
    private bool CanViewEscalation => HasAnyRole(RoleNames.QAManager, RoleNames.QASupportManager, RoleNames.SMT, RoleNames.SystemSupport);

    protected override Task OnParametersSetAsync()
    {
        if (CurrentUser is null)
        {
            return Task.CompletedTask;
        }

        if (_lastTenantId == TenantId && _lastUserId == CurrentUser.UserId)
        {
            return Task.CompletedTask;
        }

        _lastTenantId = TenantId;
        _lastUserId = CurrentUser.UserId;

        if (_isLoadingCounts)
        {
            _reloadRequested = true;
            return Task.CompletedTask;
        }

        _ = LoadCounts();

        return Task.CompletedTask;
    }

    private async Task LoadCounts()
    {
        if (_isLoadingCounts)
        {
            _reloadRequested = true;
            return;
        }

        _isLoadingCounts = true;

        try
        {
            _reloadRequested = false;
            _loading = true;
            _errorMessage = null;
            await InvokeAsync(StateHasChanged);

            Task<int>? enrolmentQa1Task = null;
            Task<int>? enrolmentQa2Task = null;
            Task<int>? enrolmentEscalationTask = null;
            Task<int>? activityQa1Task = null;
            Task<int>? activityQa2Task = null;
            Task<int>? activityEscalationTask = null;

            if (ShowEnrolments)
            {
                if (CanViewFirstPass)
                {
                    enrolmentQa1Task = GetEnrolmentQa1Count();
                }

                if (CanViewSecondPass)
                {
                    enrolmentQa2Task = GetEnrolmentQa2Count();
                }

                if (CanViewEscalation)
                {
                    enrolmentEscalationTask = GetEnrolmentEscalationCount();
                }
            }

            if (ShowActivities)
            {
                if (CanViewFirstPass)
                {
                    activityQa1Task = GetActivityQa1Count();
                }

                if (CanViewSecondPass)
                {
                    activityQa2Task = GetActivityQa2Count();
                }

                if (CanViewEscalation)
                {
                    activityEscalationTask = GetActivityEscalationCount();
                }
            }

            var allTasks = new Task<int>?[]
            {
                enrolmentQa1Task,
                enrolmentQa2Task,
                enrolmentEscalationTask,
                activityQa1Task,
                activityQa2Task,
                activityEscalationTask
            };

            await Task.WhenAll(allTasks.OfType<Task<int>>());

            _enrolmentQa1Count = enrolmentQa1Task?.Result ?? 0;
            _enrolmentQa2Count = enrolmentQa2Task?.Result ?? 0;
            _enrolmentEscalationCount = enrolmentEscalationTask?.Result ?? 0;
            _activityQa1Count = activityQa1Task?.Result ?? 0;
            _activityQa2Count = activityQa2Task?.Result ?? 0;
            _activityEscalationCount = activityEscalationTask?.Result ?? 0;
        }
        catch
        {
            _errorMessage = "Unable to fetch queue summary.";
        }
        finally
        {
            _loading = false;
            _isLoadingCounts = false;
            await InvokeAsync(StateHasChanged);

            if (_reloadRequested)
            {
                _ = LoadCounts();
            }
        }
    }

    private async Task<int> GetEnrolmentQa1Count()
    {
        var result = await GetNewMediator().Send(new Qa1WithPagination.Query
        {
            CurrentUser = CurrentUser,
            TenantId = TenantId,
            PageNumber = 1,
            PageSize = 1,
            OrderBy = "Created",
            SortDirection = "Descending"
        });

        return result is { Succeeded: true, Data: not null } ? result.Data.TotalItems : 0;
    }

    private async Task<int> GetEnrolmentQa2Count()
    {
        var result = await GetNewMediator().Send(new Qa2WithPagination.Query
        {
            CurrentUser = CurrentUser,
            TenantId = TenantId,
            PageNumber = 1,
            PageSize = 1,
            OrderBy = "Created",
            SortDirection = "Descending"
        });

        return result is { Succeeded: true, Data: not null } ? result.Data.TotalItems : 0;
    }

    private async Task<int> GetEnrolmentEscalationCount()
    {
        var result = await GetNewMediator().Send(new QaEscalationWithPagination.Query
        {
            CurrentUser = CurrentUser,
            TenantId = TenantId,
            PageNumber = 1,
            PageSize = 1,
            OrderBy = "Created",
            SortDirection = "Descending"
        });

        return result is { Succeeded: true, Data: not null } ? result.Data.TotalItems : 0;
    }

    private async Task<int> GetActivityQa1Count()
    {
        var result = await GetNewMediator().Send(new ActivityQa1WithPagination.Query
        {
            CurrentUser = CurrentUser,
            TenantId = TenantId,
            PageNumber = 1,
            PageSize = 1,
            OrderBy = "Created",
            SortDirection = "Descending"
        });

        return result is { Succeeded: true, Data: not null } ? result.Data.TotalItems : 0;
    }

    private async Task<int> GetActivityQa2Count()
    {
        var result = await GetNewMediator().Send(new ActivityQa2WithPagination.Query
        {
            CurrentUser = CurrentUser,
            TenantId = TenantId,
            PageNumber = 1,
            PageSize = 1,
            OrderBy = "Created",
            SortDirection = "Descending"
        });

        return result is { Succeeded: true, Data: not null } ? result.Data.TotalItems : 0;
    }

    private async Task<int> GetActivityEscalationCount()
    {
        var result = await GetNewMediator().Send(new ActivityQaEscalationWithPagination.Query
        {
            CurrentUser = CurrentUser,
            TenantId = TenantId,
            PageNumber = 1,
            PageSize = 1,
            OrderBy = "Created",
            SortDirection = "Descending"
        });

        return result is { Succeeded: true, Data: not null } ? result.Data.TotalItems : 0;
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