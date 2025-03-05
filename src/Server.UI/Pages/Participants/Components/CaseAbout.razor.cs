using Cfo.Cats.Application.Features.Activities.Queries;
using Cfo.Cats.Application.Features.Participants.Commands;
using Cfo.Cats.Application.Features.Participants.DTOs;
using Cfo.Cats.Application.Features.Participants.Queries;
using Cfo.Cats.Server.UI.Pages.Activities;
using MassTransit.RabbitMqTransport.Topology;

namespace Cfo.Cats.Server.UI.Pages.Participants.Components;

public partial class CaseAbout
{
    [Parameter, EditorRequired]
    public required string ParticipantId { get; set; }

    ParticipantDto? participant;
    IEnumerable<ParticipantContactDetailDto> contactDetails = [];

    protected override async Task OnInitializedAsync()
    {
        await Refresh();
        await base.OnInitializedAsync();
    }

    async Task Refresh()
    {
        participant = await GetNewMediator().Send(new GetParticipantById.Query()
        {
            Id = ParticipantId
        });

        contactDetails = await GetNewMediator().Send(new GetContactDetails.Query()
        {
            ParticipantId = ParticipantId
        });
    }

    async Task AddContact()
    {
        var parameters = new DialogParameters<AddressDialog>()
        {
            {
                x => x.Model, new AddContactDetail.Command()
                {
                    ParticipantId = ParticipantId
                }
            }
        };

        var options = new DialogOptions
        {
            MaxWidth = MaxWidth.Small,
            FullWidth = true,
            CloseButton = true,
            BackdropClick = false
        };

        var dialog = await DialogService.ShowAsync<AddressDialog>("Add Contact Details", parameters, options);

        if(await dialog.Result is not { Canceled: true })
        {
            await Refresh();
        }
    }

    Task Edit() => Task.CompletedTask;
}
