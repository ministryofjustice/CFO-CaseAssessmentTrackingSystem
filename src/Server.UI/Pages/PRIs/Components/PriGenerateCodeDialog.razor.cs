using Cfo.Cats.Application.Features.Participants.Commands;

namespace Cfo.Cats.Server.UI.Pages.PRIs.Components;

public partial class PriGenerateCodeDialog
{
    private MudForm? form = new();

    [CascadingParameter]
    private IMudDialogInstance MudDialog { get; set; } = default!;

    [EditorRequired]
    [Parameter]
    public UpsertPriCode.Command? Model { get; set; }

    private bool PRICodeGenerated = false;

    private string _code = string.Empty;

    public async Task GeneratePriCode()
    {
        await form!.Validate();

        if (form.IsValid is false)
        {
            return;
        }

        var result = await GetNewMediator().Send(Model!);

        if (result.Succeeded)
        {
            _code = result.Data.ToString();
            PRICodeGenerated = true;
            Snackbar.Add($"{L["PRI code: " + _code + " successfully generated"]}", Severity.Info);
        }
        else
        {
            Snackbar.Add($"{result.ErrorMessage}", Severity.Error);
        }
    }

    private void Cancel()
    {
        MudDialog.Cancel();
    }

    private void Close() => MudDialog.Close(DialogResult.Ok(true));
}