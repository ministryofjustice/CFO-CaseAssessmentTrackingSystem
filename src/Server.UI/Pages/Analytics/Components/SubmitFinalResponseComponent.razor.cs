using Cfo.Cats.Application.Features.PerformanceManagement.Commands;
using Cfo.Cats.Application.SecurityConstants;
using Cfo.Cats.Domain.Common.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components.Authorization;

namespace Cfo.Cats.Server.UI.Pages.Analytics.Components;
public partial class SubmitFinalResponseComponent
{
    private MudForm _form = new();
    private bool ReadOnly { get; set; } = true;

    [Inject] private IAuthorizationService AuthorizationService { get; set; } = default!;
    [Inject] private AuthenticationStateProvider AuthenticationStateProvider { get; set; } = default!;

    [Parameter, EditorRequired] public required DipSampleStatus Status { get; set; }
    [CascadingParameter] private Task<AuthenticationState> AuthState { get; set; } = default!;

    [Parameter][EditorRequired] public required EventCallback<SubmitFinalResponse.Command> OnFormSubmit { get; set; }
    [Parameter][EditorRequired] public required SubmitFinalResponse.Command Command { get; set; }

    protected override async Task OnInitializedAsync()
    {
        if (Status == DipSampleStatus.Verified)
        {
            var state = await AuthState;
            var result = await AuthorizationService.AuthorizeAsync(state.User, SecurityPolicies.OutcomeQualityDipFinalise);
            ReadOnly = result is not { Succeeded: true };
        }
        else
        {
            ReadOnly = true;
        }
    }

    private async Task OnSubmit()
    {
        await _form.Validate();

        if (_form.IsValid)
        {
            await OnFormSubmit.InvokeAsync(Command);
        }
    }

}