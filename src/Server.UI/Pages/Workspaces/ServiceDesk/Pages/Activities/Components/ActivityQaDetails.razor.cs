using Cfo.Cats.Application.Features.Activities.DTOs;
using Cfo.Cats.Application.Features.Participants.Queries;
using Cfo.Cats.Infrastructure.Constants;

namespace Cfo.Cats.Server.UI.Pages.Workspaces.ServiceDesk.Pages.Activities.Components;

public partial class ActivityQaDetails
{
    [Parameter, EditorRequired] public ActivityQaDetailsDto Activity { get; set; } = null!;

    [Parameter]
    public string? WorkspaceRef { get; set; }

    private bool _hasParticipantBeenAtThisLocationOnThisDate;

    private readonly string _licenceEndedWarningMessage = ConstantString.LicenceEndedWarning;

    private DateOnly? PostLicenceCaseClosureEnd =>
        Activity.Participant?.DeactivatedInFeed?.AddDays(30);

    protected override async Task OnInitializedAsync()
    {
        await CheckParticipantBeenAtThisLocationOnDate();
        await base.OnInitializedAsync();
    }

    private async Task CheckParticipantBeenAtThisLocationOnDate() =>
        _hasParticipantBeenAtThisLocationOnThisDate = await GetNewMediator().Send(
            new GetParticipantWasAtThisLocationCheck.Query()
            {
                ParticipantId = Activity.ParticipantId,
                LocationId = Activity.Location!.Id,
                DateAtLocation = Activity.CommencedOn
            });
}