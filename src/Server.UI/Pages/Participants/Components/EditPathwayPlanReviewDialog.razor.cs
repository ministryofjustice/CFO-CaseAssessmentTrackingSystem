using Cfo.Cats.Application.Features.Locations.DTOs;
using Cfo.Cats.Application.Features.PathwayPlans.Commands;

namespace Cfo.Cats.Server.UI.Pages.Participants.Components;

public partial class EditPathwayPlanReviewDialog
{
    private MudForm? _form;
    private bool _saving;
    private LocationDto? _selectedLocation;
    [CascadingParameter] private IMudDialogInstance MudDialog { get; set; } = null!;
    [Parameter, EditorRequired] public required EditPathwayPlanReview.Command Model { get; set; }

    protected override void OnInitialized()
    {
        _selectedLocation = Locations
            .GetVisibleLocations(CurrentUser.TenantId!)
            .FirstOrDefault(x => x.Id == Model.LocationId);
        
        base.OnInitialized();
    }

    private void Cancel() => MudDialog.Cancel();

    private async Task Submit()
    {
        if (_form is null)
        {
            return;
        }

        try
        {
            _saving = true;

            await _form.Validate();

            if (!_form.IsValid)
            {
                return;
            }

            if (_selectedLocation is not null)
            {
                Model.LocationId = _selectedLocation.Id;
            }

            MudDialog.Close(DialogResult.Ok(true));
        }
        finally
        {
            _saving = false;
        }
    }
}