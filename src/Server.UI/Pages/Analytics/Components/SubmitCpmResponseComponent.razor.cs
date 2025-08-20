using Cfo.Cats.Application.Features.PerformanceManagement.Commands;
using Cfo.Cats.Application.SecurityConstants;
using Cfo.Cats.Domain.Common.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components.Authorization;

namespace Cfo.Cats.Server.UI.Pages.Analytics.Components;

public partial class SubmitCpmResponseComponent
{
    private MudForm form = new();
    private bool ReadOnly { get; set; } = true;

    [Parameter][EditorRequired] public required EventCallback<SubmitCpmResponse.Command> OnFormSubmit { get; set; }
    [Parameter][EditorRequired] public required SubmitCpmResponse.Command Command { get; set; }
    [CascadingParameter] private Task<AuthenticationState> AuthState { get; set; } = default!;
    [Parameter, EditorRequired] public required DipSampleStatus Status { get; set; }

    private async Task OnSubmit()
    {
        await form.Validate();

        if (form.IsValid)
        {
            await OnFormSubmit.InvokeAsync(Command);
        }
    }

    protected override async Task OnInitializedAsync()
    {
        if (Status == DipSampleStatus.Reviewed)
        {
            var state = await AuthState;
            var result =
                await AuthorizationService.AuthorizeAsync(state.User, SecurityPolicies.OutcomeQualityDipVerification);
            ReadOnly = result is not { Succeeded: true };
        }
        else
        {
            ReadOnly = true;
        }
    }
}
