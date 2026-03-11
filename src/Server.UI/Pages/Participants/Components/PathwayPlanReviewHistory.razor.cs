using Cfo.Cats.Application.Features.PathwayPlans.Commands;
using Cfo.Cats.Application.Features.PathwayPlans.DTOs;
using Cfo.Cats.Application.Features.PathwayPlans.Queries;

namespace Cfo.Cats.Server.UI.Pages.Participants.Components;

public partial class PathwayPlanReviewHistory
{
    protected override IRequest<Result<PathwayPlanReviewHistoryDto[]>> CreateQuery()
        => new GetPathwayPlanReviewHistory.Query()
        {
            CurrentUser = CurrentUser,
            ParticipantId = ParticipantId
        };

    private async Task EditPathwayPlanReview(PathwayPlanReviewHistoryDto pathwayPlanReview)
    {
        var command = new EditPathwayPlanReview.Command
        {
            ReviewId = pathwayPlanReview.Id,
            ReviewDate = pathwayPlanReview.ReviewDate,
            Review = pathwayPlanReview.Review,
            ReviewReason = pathwayPlanReview.ReviewReason,
            LocationId = pathwayPlanReview.LocationId
        };

        var parameters = new DialogParameters<EditPathwayPlanReviewDialog>
        {
            { x => x.Model, command }
        };

        var options = new DialogOptions
        {
            CloseButton = true,
            MaxWidth = MaxWidth.Small,
            FullWidth = true
        };

        var dialog = await DialogService
            .ShowAsync<EditPathwayPlanReviewDialog>(
                "Edit Pathway Plan Review",
                parameters,
                options);

        if (await dialog.Result is { Canceled: false })
        {
            var result = await Service.Send(command);

            if (result.Succeeded)
            {
                Snackbar.Add("Review updated successfully.", Severity.Success);
                await RefreshAsync();
            }
            else
            {
                Snackbar.Add(result.ErrorMessage, Severity.Error);
            }
        }
    }
}