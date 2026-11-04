using Cfo.Cats.Application.Features.Announcements.Commands;
using Cfo.Cats.Application.Features.Announcements.DTOs;
using Cfo.Cats.Application.Features.Announcements.Queries;

namespace Cfo.Cats.Server.UI.Pages.Announcements;

public partial class ManageAnnouncementTypesDialog
{
    [CascadingParameter] private IMudDialogInstance MudDialog { get; set; } = null!;
    private readonly AddAnnouncementType.Command _model = new();
    private AnnouncementTypeDto[] _types = [];
    private AnnouncementTypeDto? _editing;
    private MudForm? _form;
    private bool _busy;
    private string? _error;

    protected override Task OnInitializedAsync() => RefreshAsync();

    private async Task LoadTypesAsync()
    {
        var result = await GetNewMediator().Send(new GetAnnouncementTypes.Query());
        if (result is { Succeeded: true, Data: not null })
        {
            _types = result.Data;
        }
        else
        {
            _error = result.ErrorMessage;
        }
    }

    private async Task RefreshAsync()
    {
        _busy = true;
        _error = null;
        CancelEdit();
        try
        {
            await LoadTypesAsync();
        }
        finally
        {
            _busy = false;
        }
    }

    private void Edit(AnnouncementTypeDto type)
    {
        _editing = type;
        _model.Name = type.Name;
        _error = null;
    }

    private void CancelEdit()
    {
        _editing = null;
        _model.Name = string.Empty;
    }

    private async Task SaveAsync()
    {
        await _form!.ValidateAsync();
        if (!_form.IsValid)
        {
            return;
        }
        ICommand<Result> command = _editing is null ? _model : new RenameAnnouncementType.Command
        {
            Id = _editing.Id, OriginalName = _editing.Name, Name = _model.Name
        };
        await RunCommandAsync(command, _editing is null ? "Announcement type added." : "Announcement type updated.");
    }

    private Task DeleteAsync(AnnouncementTypeDto type)
        => DialogServiceHelper.ShowConfirmationDialog(
            "Delete announcement type",
            $"Delete '{type.Name}'? It will no longer be available for new revisions. Existing announcements will keep their original type.",
            () => RunCommandAsync(new DeleteAnnouncementType.Command
            {
                Id = type.Id, OriginalName = type.Name
            }, "Announcement type deleted."));

    private async Task RunCommandAsync(ICommand<Result> command, string successMessage)
    {
        _busy = true;
        _error = null;
        try
        {
            var result = await GetNewMediator().Send(command);
            if (result.Succeeded)
            {
                Snackbar.Add(successMessage, Severity.Success);
                CancelEdit();
                await LoadTypesAsync();
            }
            else
            {
                _error = result.ErrorMessage;
            }
        }
        finally
        {
            _busy = false;
        }
    }
}
