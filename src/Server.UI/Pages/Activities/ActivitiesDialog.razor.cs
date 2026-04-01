using Cfo.Cats.Application.Features.Activities.Commands;
using Cfo.Cats.Application.Features.Activities.DTOs;
using Cfo.Cats.Application.Features.Activities.Queries;
using Cfo.Cats.Application.Features.PathwayPlans.DTOs;
using Cfo.Cats.Domain.Common.Enums;
using Cfo.Cats.Server.UI.Pages.Activities.Components;

namespace Cfo.Cats.Server.UI.Pages.Activities;

public partial class ActivitiesDialog
{
    private PaginatedData<ActivitySummaryDto>? _activities;

    [Parameter, EditorRequired] public required ActivitiesWithPagination.Query Model { get; set; }

    [Parameter, EditorRequired] public required ObjectiveDto Objective { get; set; }

    [CascadingParameter] public required IMudDialogInstance Dialog { get; set; }

    [Parameter] public ObjectiveTaskDto? Task { get; set; }

    protected override async Task OnInitializedAsync()
    {
        await OnRefresh();
        await base.OnInitializedAsync();
    }

    private async Task OnRefresh()
    {
        Model.PageSize = 5;
        Model.OrderBy = "Created";
        Model.SortDirection = $"{SortDirection.Descending}";

        _activities = await GetNewMediator().Send(Model);
    }

    private Task OnPaginationChanged(int arg)
    {
        Model.PageNumber = arg;
        return OnRefresh();
    }

    private Task OnDateRangeChanged(DateRange range)
    {
        Model.CommencedStart = range.Start;
        Model.CommencedEnd = range.End;
        return OnRefresh();
    }

    private Task OnActivityTypesChanged(IReadOnlyCollection<ActivityType>? types)
    {
        Model.IncludeTypes = types?.ToList();
        return OnRefresh();
    }

    private Task OnExcludeNotInProgress(bool exclude)
    {
        Model.Status = exclude ? ActivityStatus.PendingStatus : null;
        return OnRefresh();
    }

    private async Task Edit(ActivitySummaryDto activity)
    {
        Dialog.Close();

        var parameters = new DialogParameters<EditActivityDialog>()
        {
            {
                x => x.ActivityId, activity.Id
            }
        };

        var dialog = await DialogService.ShowAsync<EditActivityDialog>("Edit Activity/ETE", parameters,
            new DialogOptions
            {
                MaxWidth = MaxWidth.Small,
                FullWidth = true,
                CloseButton = true
            });

        var result = await dialog.Result;

        if (result!.Canceled == false)
        {
            await OnRefresh();
        }
    }

    private async Task Abandon(ActivitySummaryDto activity)
    {
        Dialog.Close();

        var parameters = new DialogParameters<AbandonActivityDialog>()
        {
            {
                x => x.model, new AbandonActivity.Command()
                {
                    ActivityId = activity.Id,
                    AbandonJustification = "",
                    AbandonReason = ActivityAbandonReason.Other,
                    AbandonedBy = CurrentUser.UserId!
                }
            }
        };

        var options = new DialogOptions
            { CloseButton = true, MaxWidth = MaxWidth.Small, FullWidth = true, BackdropClick = false };

        var dialog = await DialogService.ShowAsync<AbandonActivityDialog>("Abandon Activity/ETE", parameters, options);

        var result = await dialog.Result;

        if (result!.Canceled == false)
        {
            await OnRefresh();
        }
    }
}