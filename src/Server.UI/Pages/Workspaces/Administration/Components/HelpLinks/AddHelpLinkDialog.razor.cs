using Cfo.Cats.Application.Features.HelpLinks.Commands.AddHelpLink;
using Cfo.Cats.Application.Features.HelpLinks.DTOs;
using Cfo.Cats.Domain.HelpLinks;
using Cfo.Cats.Infrastructure.Constants;

namespace Cfo.Cats.Server.UI.Pages.Workspaces.Administration.Components.HelpLinks;

public partial class AddHelpLinkDialog
{
    private MudForm? _form;
    private bool _saving;

    [CascadingParameter]
    private IMudDialogInstance MudDialog { get; set; } = null!;

    [Parameter, EditorRequired]
    public AddHelpLinkCommand Model { get; set; } = null!;

    private void Cancel() => MudDialog.Cancel();

    private void AddUrl() => Model.Urls.Add(new HelpLinkUrlDto());

    private void RemoveUrl(int index)
    {
        if (Model.Urls.Count > 1)
        {
            Model.Urls.RemoveAt(index);
        }
    }

    private async Task Add()
    {
        try
        {
            _saving = true;

            await _form!.ValidateAsync();

            if (_form!.IsValid == false)
            {
                return;
            }

            var result = await Service.Send(Model);

            if (result.Succeeded)
            {
                MudDialog.Close(DialogResult.Ok(true));
                Snackbar.Add(ConstantString.SaveSuccess, Severity.Info);
            }
            else
            {
                Snackbar.Add(message: result.ErrorMessage, Severity.Error);
            }
        }
        finally
        {
            _saving = false;
        }
    }
}
