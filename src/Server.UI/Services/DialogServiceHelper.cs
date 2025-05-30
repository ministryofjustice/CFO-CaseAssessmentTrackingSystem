using Cfo.Cats.Server.UI.Components.Dialogs;

namespace Cfo.Cats.Server.UI.Services;

public class DialogServiceHelper
{
    private readonly IDialogService _dialogService;
    private readonly ISnackbar _snackbar;
    
    public DialogServiceHelper(IDialogService dialogService, ISnackbar snackbar)
    {
        _dialogService = dialogService;
        _snackbar = snackbar;
    }
    
    public async Task ShowDeleteConfirmationDialog(IRequest<Result<int>> command,string title, string contentText, Func<Task> onConfirm, Func<Task>? onCancel = null)
    {
        var parameters = new DialogParameters
        {
            { nameof(DeleteConfirmation.ContentText), contentText },
            { nameof(DeleteConfirmation.Command), command }
        };

        var options = new DialogOptions { CloseButton = true, MaxWidth = MaxWidth.Small, FullWidth = true };
        var dialog = await _dialogService.ShowAsync<DeleteConfirmation>(title, parameters, options);
        var result = await dialog.Result;
        if (!result!.Canceled)
        {
            await onConfirm();
        }
        else if (onCancel != null)
        {
            await onCancel();
        }
    }

    public async Task ShowConfirmationDialog(string title, string contentText, Func<Task> onConfirm, Func<Task>? onCancel = null)
    {
        var parameters = new DialogParameters
        {
            { nameof(ConfirmationDialog.ContentText), contentText }
        };

        var options = new DialogOptions { CloseButton = true, MaxWidth = MaxWidth.Small, FullWidth = true };
        var dialog = await _dialogService.ShowAsync<ConfirmationDialog>(title, parameters, options);
        var result = await dialog.Result;

        if (!result!.Canceled)
        {
            await onConfirm();
        }
        else if (onCancel != null)
        {
            await onCancel();
        }
    }
}
