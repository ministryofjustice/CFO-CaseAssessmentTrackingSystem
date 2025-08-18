using Cfo.Cats.Application.Features.ManagementInformation.Commands;

namespace Cfo.Cats.Server.UI.Pages.Analytics.Components;

public partial class SubmitCpmResponseComponent
{
    private MudForm form = new();

    [Parameter][EditorRequired] public required EventCallback<AddOutcomeQualityDipSampleCpm.Command> OnFormSubmit { get; set; }
    [Parameter][EditorRequired] public required AddOutcomeQualityDipSampleCpm.Command Command { get; set; }

    private async Task OnSubmit()
    {
        await form.Validate();

        if (form.IsValid)
        {
            await OnFormSubmit.InvokeAsync(Command);
        }
    }

}
