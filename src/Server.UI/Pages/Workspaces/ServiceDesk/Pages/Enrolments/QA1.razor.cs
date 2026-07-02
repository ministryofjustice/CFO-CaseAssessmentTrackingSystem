using Cfo.Cats.Application.Common.Security;
using Cfo.Cats.Application.Features.Assessments.DTOs;
using Cfo.Cats.Application.Features.Assessments.Queries;
using Cfo.Cats.Application.Features.Participants.DTOs;
using Cfo.Cats.Application.Features.Participants.Queries;
using Cfo.Cats.Application.Features.QualityAssurance.Commands;
using Cfo.Cats.Application.Features.QualityAssurance.DTOs;
using Cfo.Cats.Server.UI.Pages.Workspaces.ServiceDesk.Pages.Enrolments.Components;
using Microsoft.EntityFrameworkCore;
using System.Text;
using IResult = Cfo.Cats.Application.Common.Interfaces.IResult;

namespace Cfo.Cats.Server.UI.Pages.Workspaces.ServiceDesk.Pages.Enrolments;

public partial class QA1
{
    private int CharacterCount => Command.Message?.Length ?? 0;
    private QaExternalMessageWarning? _warningMessage;
    private MudForm? _form;
    private EnrolmentQueueEntryDto? _queueEntry = null;
    private ParticipantDto? _participantDto = null;
    private ParticipantAssessmentDto? _latestParticipantAssessmentDto;
    private bool _saving = false;
    private bool _loadingQueueItem;

    [CascadingParameter]
    public UserProfile? UserProfile { get; set; } = null!;

    [SupplyParameterFromQuery]
    public Guid? QueueEntryId { get; set; }

    private SubmitQa1Response.Command Command { get; set; } = default!;

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

            var entry = await uow.DbContext.EnrolmentQa1Queue
                .Include(q => q.SupportWorker)
                .FirstOrDefaultAsync(x => x.Id == queueEntryId, ComponentCancellationToken);

            if (entry is null)
            {
                Snackbar.Add("Queue item not found.", Severity.Info);
                return;
            }

            _queueEntry = Mapper.Map<EnrolmentQueueEntryDto>(entry);

            _participantDto = await GetNewMediator().Send(new GetParticipantById.Query()
            {
                Id = _queueEntry.ParticipantId
            });

            Command = new SubmitQa1Response.Command()
            {
                QueueEntryId = _queueEntry.Id,
                CurrentUser = UserProfile!
            };

            await SetLatestParticipantAssessment(_queueEntry.ParticipantId);
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

            GrabQa1Entry.Command command = new GrabQa1Entry.Command()
            {
                CurrentUser = UserProfile!
            };

            var result = await GetNewMediator().Send(command);

            if (result.Succeeded)
            {
                _queueEntry = result.Data!;

                _participantDto = await GetNewMediator().Send(new GetParticipantById.Query()
                {
                    Id = _queueEntry.ParticipantId
                });

                Command = new SubmitQa1Response.Command()
                {
                    QueueEntryId = _queueEntry.Id,
                    CurrentUser = UserProfile
                };

                await SetLatestParticipantAssessment(_queueEntry.ParticipantId);
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

    protected async Task SetLatestParticipantAssessment(string participantId)
    {
        if (!string.IsNullOrEmpty(participantId))
        {
            var query = new GetAssessmentScores.Query()
            {
                ParticipantId = participantId
            };

            var result = await GetNewMediator().Send(query);

            if (result.Succeeded && result.Data != null)
            {
                _latestParticipantAssessmentDto = result.Data.MaxBy(pa => pa.CreatedDate);
            }
        }
    }

    protected async Task SubmitToQa()
    {
        try
        {
            _saving = true;

            await _form!.ValidateAsync();

            if (_form.IsValid is false)
            {
                return;
            }

            var result = await GetNewMediator().Send(Command);

            if (result.Succeeded)
            {
                Snackbar.Add("Participant submitted to QA2", Severity.Info);
                Navigation.NavigateTo("/pages/workspace/servicedesk", true);
            }
            else
            {
                ShowActionFailure("Failed to submit participant to QA2", result);
            }
        }
        finally { _saving = false; }
    }

    private void ShowActionFailure(string title, IResult result)
    {
        var html = new StringBuilder();

        html.Append($"<div>");
        html.Append($"<h2>{title}</h2>");
        html.Append($"<ul>");
        foreach (var e in result.Errors)
        {
            html.Append($"<li>{e}</li>");
        }
        html.Append($"</ul>");
        html.Append($"</div>");

        RenderFragment content = builder =>
        {
            builder.AddMarkupContent(0, html.ToString());
        };

        Snackbar.Add(content, Severity.Error, options =>
        {
            options.RequireInteraction = true;
            options.SnackbarVariant = Variant.Text;
        });
    }
}