using AutoMapper;
using Cfo.Cats.Application.Features.Participants.Commands;
using Cfo.Cats.Application.Features.Participants.DTOs;
using Cfo.Cats.Application.Features.Participants.Queries;
using Cfo.Cats.Infrastructure.Constants;
using Cfo.Cats.Server.UI.Components.Dialogs;

namespace Cfo.Cats.Server.UI.Pages.Participants.Components;

public partial class CaseAbout(IMapper mapper)
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
        => await AddOrEditContact(new AddOrEditContactDetail.Command { ParticipantId = ParticipantId }, "Add Contact Details");

    async Task EditContact(ParticipantContactDetailDto contact) =>
        await AddOrEditContact(mapper.Map<AddOrEditContactDetail.Command>(contact), "Edit Contact Details");

    async Task AddOrEditContact(AddOrEditContactDetail.Command command, string title)
    {
        var parameters = new DialogParameters<AddressDialog>()
        {
            { x => x.Model, command }
        };

        var options = new DialogOptions { MaxWidth = MaxWidth.Small, FullWidth = true, CloseButton = true, BackdropClick = false };

        var dialog = await DialogService.ShowAsync<AddressDialog>(title, parameters, options);

        if (await dialog.Result is not { Canceled: true })
        {
            await Refresh();
        }
    }

    async Task DeleteContact(ParticipantContactDetailDto contact)
    {
        var parameters = new DialogParameters<DeleteConfirmation>
        {
            { x => x.Command, new DeleteContactDetail.Command() { ContactDetailId = contact.Id } },
            { x => x.ContentText, string.Format(ConstantString.DeleteConfirmation, contact.Description) }
        };

        var options = new DialogOptions { CloseButton = true, MaxWidth = MaxWidth.Small, FullWidth = true };

        var dialog = await DialogService.ShowAsync<DeleteConfirmation>("Delete Confirmation", parameters, options);

        if (await dialog.Result is not { Canceled: true })
        {
            await Refresh();
        }
    }

    async Task EditPersonalInformation() => await Task.CompletedTask;
}
