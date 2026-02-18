using Cfo.Cats.Application.Features.Participants.Commands;

namespace Cfo.Cats.Server.UI.Pages.PRIs.Components;

public partial class PriGenerateCodeDialog
{
    private bool _loading = false;
    private MudForm? _form = new();
    private bool _priCodeGenerated = false;

    [CascadingParameter]
    private IMudDialogInstance MudDialog { get; set; } = default!;

    [EditorRequired]
    [Parameter]
    public UpsertPriCode.Command? Model { get; set; }
    
    private string _code = string.Empty;

    public async Task GeneratePriCode()
    {
        try
        {
            _loading = true;

            await _form!.Validate();

            if (_form.IsValid is false)
            {
                return;
            }

            var result = await GetNewMediator().Send(Model!);

            if (result.Succeeded)
            {
                _code = result.Data.ToString();
                _priCodeGenerated = true;
                Snackbar.Add($"{L["PRI code: " + _code + " successfully generated"]}", Severity.Info);
            }
            else
            {
                Snackbar.Add($"{result.ErrorMessage}", Severity.Error);
            }
        }
        finally
        {
            _loading = false;    
        }
    }

    private void Cancel() => MudDialog.Cancel();

    private void Close() => MudDialog.Close(DialogResult.Ok(true));
}