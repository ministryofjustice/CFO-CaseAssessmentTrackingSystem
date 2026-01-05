using Cfo.Cats.Application.Common.Security;
using Cfo.Cats.Application.Features.Activities.Commands;
using Cfo.Cats.Application.Features.Activities.DTOs;
using Cfo.Cats.Application.Features.Activities.Queries;
using Cfo.Cats.Server.UI.Pages.QA.Activities.Components;
using IResult = Cfo.Cats.Application.Common.Interfaces.IResult;

namespace Cfo.Cats.Server.UI.Pages.QA.Activities;

public partial class QA2 : CatsComponentBase
{
    private ActivityQaExternalMessageWarning? _warningMessage;
    private MudForm? _form;
    private ActivityQueueEntryDto? _queueEntry;
    private ActivityQaDetailsDto? _activityQaDetailsDto;

    [CascadingParameter]
    public UserProfile? UserProfile { get; set; }

    protected SubmitActivityQa2Response.Command Command { get; set; } = null!;

    protected async Task GetQueueItem()
    {
        var command = new GrabActivityQa2Entry.Command
        {
            CurrentUser = UserProfile!
        };

        var result = await GetNewMediator().Send(command);

        if (result.Succeeded)
        {
            _queueEntry = result.Data!;

            var activity = await GetNewMediator().Send(
                new GetActivityById.Query
                {
                    Id = _queueEntry.ActivityId
                });

            _activityQaDetailsDto = Mapper.Map<ActivityQaDetailsDto>(activity);
            _activityQaDetailsDto.ActivityId = activity!.Id;

            Command = new SubmitActivityQa2Response.Command
            {
                ActivityQueueEntryId = _queueEntry.Id,
                CurrentUser = UserProfile!
            };
        }
        else
        {
            Snackbar.Add(result.ErrorMessage, Severity.Info);
        }
    }

    private async Task SubmitToQa()
    {
        await _form!.Validate();

        if (_form.IsValid is false)
        {
            return;
        }

        var submit = true;

        if (Command is { IsMessageExternal: true, Message.Length: > 0 })
        {
            submit = await _warningMessage!.ShowAsync();
        }

        if (submit)
        {
            var result = await GetNewMediator().Send(Command);

            if (result.Succeeded)
            {
                Snackbar.Add("Activity submitted", Severity.Info);
                Navigation.NavigateTo("/pages/qa/activities/qa2", true);
            }
            else
            {
                ShowActionFailure("Failed to submit", result);
            }
        }
    }

    private void ShowActionFailure(string title, IResult result) =>
        Snackbar.Add(
            RenderFailure(title, result),
            Severity.Error,
            options =>
            {
                options.RequireInteraction = true;
                options.SnackbarVariant = Variant.Text;
            });
}