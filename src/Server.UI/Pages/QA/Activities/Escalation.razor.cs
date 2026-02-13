using Cfo.Cats.Application.Common.Security;
using Cfo.Cats.Application.Features.Activities.Commands;
using Cfo.Cats.Application.Features.Activities.DTOs;
using Cfo.Cats.Application.Features.Activities.Queries;
using Cfo.Cats.Server.UI.Pages.QA.Activities.Components;
using IResult = Cfo.Cats.Application.Common.Interfaces.IResult;

namespace Cfo.Cats.Server.UI.Pages.QA.Activities;

public partial class Escalation 
{
    private ActivityQaExternalMessageWarning? _warningMessage;
    private MudForm? _form;
    private ActivityQueueEntryDto? _queueEntry;
    private ActivityQaDetailsDto? _activityQaDetailsDto;

    [Parameter]
    public Guid Id { get; set; }

    [CascadingParameter]
    public UserProfile? UserProfile { get; set; }

    protected SubmitActivityEscalationResponse.Command Command { get; set; } = null!;
    
    protected override async Task OnInitializedAsync()
    {
        if (_activityQaDetailsDto is not null)
        {
            return;
        }

        var result = await GetNewMediator().Send(
            new GetActivityEscalationEntryById.Query
            {
                Id = Id,
                CurrentUser = UserProfile
            });

        if (result.Succeeded == false)
        {
            return;
        }

        _queueEntry = result.Data!;

        var activity = await GetNewMediator().Send(
            new GetActivityById.Query
            {
                Id = _queueEntry.ActivityId
            });
        
        _activityQaDetailsDto = Mapper.Map<ActivityQaDetailsDto>(activity);
        _activityQaDetailsDto.ActivityId = activity!.Id;

        Command = new SubmitActivityEscalationResponse.Command
        {
            ActivityQueueEntryId = Id,
            CurrentUser = UserProfile
        };
    }

    private async Task SubmitToQa()
    {
        await _form!.Validate();

        if (_form.IsValid == false)
        {
            return;
        }

        var submit = true;

        if (Command is { IsMessageExternal: true, Message.Length: > 0 })
        {
            submit = await _warningMessage!.ShowAsync();
        }

        if (submit == false)
        {
            return;
        }

        var result = await GetNewMediator().Send(Command);

        if (result.Succeeded)
        {
            var message = Command.Response switch
            {
                SubmitActivityEscalationResponse.EscalationResponse.Accept => "Activity accepted",
                SubmitActivityEscalationResponse.EscalationResponse.Return => "Activity returned to PQA",
                _ => "Comment added"
            };

            Snackbar.Add(message, Severity.Info);
            Navigation.NavigateTo("/pages/qa/activities/activities");
        }
        else
        {
            var message = Command.Response switch
            {
                SubmitActivityEscalationResponse.EscalationResponse.Accept => "Failed to accept activity",
                SubmitActivityEscalationResponse.EscalationResponse.Return => "Failed to return activity",
                _ => "Failed to add Comment"
            };

            ShowActionFailure(message, result);
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