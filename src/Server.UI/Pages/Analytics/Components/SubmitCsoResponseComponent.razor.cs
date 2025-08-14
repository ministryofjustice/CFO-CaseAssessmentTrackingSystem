using Cfo.Cats.Application.Features.ManagementInformation.Commands.AddOutcomeQualityDipSampleCso;
using Cfo.Cats.Application.SecurityConstants;
using Cfo.Cats.Domain.Common.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components.Authorization;

namespace Cfo.Cats.Server.UI.Pages.Analytics.Components;

public partial class SubmitCsoResponseComponent
{
    private MudForm _form = new();
    private bool ReadOnly { get; set; } = true;

    [Parameter] [EditorRequired] public required EventCallback<Command> OnFormSubmit { get; set; }
    [Parameter] [EditorRequired] public required Command Command { get; set; }
    [CascadingParameter] private Task<AuthenticationState> AuthState { get; set; } = default!;

    [Parameter, EditorRequired] public DipSampleStatus Status { get; set; } = default!;

    private async Task OnSubmit()
    {
        await _form.Validate();

        if (_form.IsValid)
        {
            await OnFormSubmit.InvokeAsync(Command);
        }
    }

    protected override async Task OnInitializedAsync()
    {
        if (Status == DipSampleStatus.AwaitingReview)
        {
            var state = await AuthState;
            var result =
                await AuthorizationService.AuthorizeAsync(state.User, SecurityPolicies.OutcomeQualityDipReview);
            ReadOnly = result is not { Succeeded: true };
        }
        else
        {
            ReadOnly = true;
        }
    }
}