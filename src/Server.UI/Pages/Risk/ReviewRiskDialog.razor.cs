using Cfo.Cats.Application.Features.Locations.DTOs;
using Cfo.Cats.Application.Features.Participants.Commands;

namespace Cfo.Cats.Server.UI.Pages.Risk;

public partial class ReviewRiskDialog
{
    private bool _saving;
    private MudForm? _form;
    [CascadingParameter] private IMudDialogInstance MudDialog { get; set; } = null!;

    [Parameter, EditorRequired] public required AddRisk.Command Model { get; set; }

    [Parameter] public bool AddReviewRequest { get; set; }

    private IEnumerable<LocationDto> _locations = [];

    protected override void OnInitialized()
    {
        _locations = Locations
            .GetVisibleLocations(CurrentUser.TenantId!)
            .ToList();

        base.OnInitialized();
    }

    private void Cancel() => MudDialog.Cancel();

    private async Task Submit()
    {
        try
        {
            _saving = true;

            if (_form is null)
            {
                _saving = false;
                return;
            }

            await _form.Validate();

            if (_form.IsValid)
            {
                MudDialog.Close(DialogResult.Ok(true));
            }
        }
        finally
        {
            _saving = false;
        }
    }
}