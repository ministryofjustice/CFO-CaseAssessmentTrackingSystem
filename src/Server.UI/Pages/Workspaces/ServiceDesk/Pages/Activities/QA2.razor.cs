using Cfo.Cats.Application.Common.Security;
using Cfo.Cats.Application.Features.Activities.Commands;
using Cfo.Cats.Application.Features.Activities.DTOs;
using Cfo.Cats.Application.Features.Activities.Queries;
using Cfo.Cats.Domain.Common.Enums;
using Cfo.Cats.Server.UI.Pages.Workspaces.ServiceDesk.Pages.Activities.Components;
using Microsoft.EntityFrameworkCore;
using IResult = Cfo.Cats.Application.Common.Interfaces.IResult;

namespace Cfo.Cats.Server.UI.Pages.Workspaces.ServiceDesk.Pages.Activities;

public partial class QA2
{   
    [Inject]
    private IPicklistService PicklistService { get; set; } = null!;
    
    private ActivityQaExternalMessageWarning? _warningMessage;
    private MudForm? _form;
    private ActivityQueueEntryDto? _queueEntry;
    private ActivityQaDetailsDto? _activityQaDetailsDto;
    private bool _loadingQueueItem;

    [CascadingParameter]
    public UserProfile? UserProfile { get; set; }

    [SupplyParameterFromQuery]
    public Guid? QueueEntryId { get; set; }

    protected SubmitActivityQa2Response.Command Command { get; set; } = null!;

    protected override async Task OnInitializedAsync()
    {
        if (QueueEntryId.HasValue)
        {
            _loadingQueueItem = true;
            await LoadQueueItem(QueueEntryId.Value);
        }
    }

    private async Task LoadQueueItem(Guid queueEntryId)
    {
        try
        {
            _loadingQueueItem = true;
            var uow = GetNewUnitOfWork();

            var entry = await uow.DbContext.ActivityQa2Queue
                .FirstOrDefaultAsync(x => x.Id == queueEntryId, ComponentCancellationToken);

            if (entry is null)
            {
                Snackbar.Add("Queue item not found.", Severity.Info);
                return;
            }

            _queueEntry = Mapper.Map<ActivityQueueEntryDto>(entry);

            var createdByDisplayName = await uow.DbContext.Users
                .Where(u => u.Id == entry.CreatedBy)
                .Select(u => u.DisplayName)
                .FirstOrDefaultAsync(ComponentCancellationToken);

            _queueEntry.Qa1CompletedBy = createdByDisplayName;

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
        finally
        {
            _loadingQueueItem = false;
        }
    }

    private async Task GetQueueItem()
    {
        try
        {
            _loadingQueueItem = true;

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
        finally
        {
            _loadingQueueItem = false;
        }
    }

    private async Task SubmitToQa()
    {
        await _form!.ValidateAsync();

        if (_form.IsValid is false)
        {
            return;
        }

        var submit = true;

        if (Command is { MessageToProvider.Length: > 0 })
        {
            submit = await _warningMessage!.ShowAsync();
        }

        if (submit)
        {
            var result = await GetNewMediator().Send(Command);

            if (result.Succeeded)
            {
                Snackbar.Add("Activity submitted", Severity.Info);
                Navigation.NavigateTo("/pages/workspace/servicedesk", true);
            }
            else
            {
                ShowActionFailure("Failed to submit", result);
            }
        }
    }

    private void OnResponseChanged()
    {
        Command.FeedbackType = null;
        Command.ReturnReason = null;
        Command.MessageToProvider = string.Empty;
        Command.MessageToQa1 = string.Empty;

        if (Command.Response == SubmitActivityQa2Response.Qa2Response.Return)
        {
            Command.FeedbackType = FeedbackType.Returned;
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