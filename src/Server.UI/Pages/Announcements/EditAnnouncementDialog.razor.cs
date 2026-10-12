using Cfo.Cats.Application.Features.Announcements.Commands;

namespace Cfo.Cats.Server.UI.Pages.Announcements;

public partial class EditAnnouncementDialog
{
    [CascadingParameter] private IMudDialogInstance MudDialog { get; set; } = null!;
    [Parameter, EditorRequired] public SaveAnnouncement.Command Model { get; set; } = null!;
    [Parameter, EditorRequired] public string[] Types { get; set; } = [];
    [Parameter] public bool TypeUnavailable { get; set; }
    [Parameter] public bool CanViewInternal { get; set; }
    [Parameter] public bool CanPublishImmediately { get; set; }

    private MudForm? _form;
    private bool _saving;
    private string? _error;

    private async Task SaveAsync()
    {
        _saving = true;
        _error = null;
        try
        {
            await _form!.ValidateAsync();
            if (!_form.IsValid)
            {
                return;
            }
            var result = await GetNewMediator().Send(Model);
            if (result.Succeeded)
            {
                Snackbar.Add(Model.PublishImmediately ? "Announcement published." : "Announcement submitted for approval.", Severity.Success);
                MudDialog.Close(DialogResult.Ok(true));
            }
            else
            {
                _error = result.ErrorMessage;
            }
        }
        finally
        {
            _saving = false;
        }
    }
}
