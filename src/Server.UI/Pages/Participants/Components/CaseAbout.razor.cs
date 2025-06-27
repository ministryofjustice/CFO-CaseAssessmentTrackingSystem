using AutoMapper;
using Cfo.Cats.Application.Features.Participants.Commands;
using Cfo.Cats.Application.Features.Participants.DTOs;
using Cfo.Cats.Application.Features.Participants.Queries;
using Cfo.Cats.Infrastructure.Constants;
using Cfo.Cats.Server.UI.Components.Dialogs;

namespace Cfo.Cats.Server.UI.Pages.Participants.Components;

public partial class CaseAbout(IMapper mapper)
{
    /// <summary>
    /// The Id of the participant
    /// </summary>
    [Parameter, EditorRequired]
    public required string ParticipantId { get; set; }
    
    /// <summary>
    /// Flag to indicate whether the participant is active or not.
    /// </summary>
    [Parameter, EditorRequired]
    public required bool ParticipantIsActive { get; set; }

    private IEnumerable<ParticipantContactDetailDto> _contactDetails = [];
    private ParticipantPersonalDetailDto? _personalDetails;
    private bool _loading = true;

    protected override Task OnInitializedAsync()
        => Refresh();

    private async Task Refresh()
    {
        try
        {
            _loading = true;
            await LoadContactDetails();
            await LoadPersonalDetails();
        }
        finally
        {
            _loading = false;
        }
    }

    private async Task LoadContactDetails()
    {
        var mediator = GetNewMediator();
        var result = await mediator.Send(new GetContactDetails.Query()
        {
            ParticipantId = ParticipantId
        }, ComponentCancellationToken);
        
        if (!IsDisposed)
        {
            _contactDetails = result.OrderByDescending(d => d.Primary);
        }
    }

    private async Task LoadPersonalDetails()
    { 
        var mediator = GetNewMediator();
        var result = await mediator.Send(new GetPersonalDetails.Query() 
        {
            ParticipantId = ParticipantId
        }, ComponentCancellationToken);

        if (!IsDisposed)
        {
            _personalDetails = result;
        }
    }

    private async Task AddContact()
        => await AddOrEditContact(new AddOrUpdateContactDetail.Command { ParticipantId = ParticipantId }, "Add Contact Details");

    private async Task EditContact(ParticipantContactDetailDto contact) =>
        await AddOrEditContact(mapper.Map<AddOrUpdateContactDetail.Command>(contact), "Edit Contact Details");

    private async Task AddOrEditContact(AddOrUpdateContactDetail.Command command, string title)
    {
        var parameters = new DialogParameters<AddressDialog>()
        {
            { x => x.Model, command }
        };

        var options = new DialogOptions { MaxWidth = MaxWidth.Small, FullWidth = true, CloseButton = true, BackdropClick = false };

        var dialog = await DialogService.ShowAsync<AddressDialog>(title, parameters, options);

        if (await dialog.Result is not null)
        {
            await Refresh();
        }
    }

    private async Task DeleteContact(ParticipantContactDetailDto contact)
    {
        var parameters = new DialogParameters<DeleteConfirmation>
        {
            { x => x.Command, new DeleteContactDetail.Command() { ContactDetailId = contact.Id } },
            { x => x.ContentText, string.Format(ConstantString.DeleteConfirmation, contact.Description) }
        };

        var options = new DialogOptions { CloseButton = true, MaxWidth = MaxWidth.Small, FullWidth = true };

        var dialog = await DialogService.ShowAsync<DeleteConfirmation>("Delete Confirmation", parameters, options);

        if (await dialog.Result is not null)
        {
            await Refresh();
        }
    }

    private async Task EditPersonalInformation()
    {
        var command = new AddOrUpdatePersonalDetail.Command { ParticipantId = ParticipantId, PersonalDetails = _personalDetails ?? new() };

        var parameters = new DialogParameters<PersonalDetailsDialog>()
        {
            { x => x.Model, command }
        };

        var options = new DialogOptions { MaxWidth = MaxWidth.Small, FullWidth = true, CloseButton = true, BackdropClick = false };

        var dialog = await DialogService.ShowAsync<PersonalDetailsDialog>("Edit Personal Details", parameters, options);

        if (await dialog.Result is not null)
        {
            await Refresh();
        }
    }
}
