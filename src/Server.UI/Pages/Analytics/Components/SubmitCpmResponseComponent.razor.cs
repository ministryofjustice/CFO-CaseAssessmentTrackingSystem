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
    private bool _showCsoComments = false;

    [Parameter][EditorRequired] public required EventCallback<SubmitCpmResponse.Command> OnFormSubmit { get; set; }
    [Parameter][EditorRequired] public required SubmitCpmResponse.Command Command { get; set; }
    [CascadingParameter] private Task<AuthenticationState> AuthState { get; set; } = default!;
    [Parameter, EditorRequired] public required DipSampleStatus Status { get; set; }
    [Parameter] public string? CsoComments { get; set; }

    private async Task CopyFromReview()
    {
        if (!string.IsNullOrWhiteSpace(Command.Comments))
        {
            bool? result = await DialogService.ShowMessageBoxAsync(
                "Confirm Copy",
                "This will replace your current comments. Do you want to continue?",
                yesText: "Yes, Replace",
                cancelText: "Cancel");
            
            if (result != true)
            {
                return;
            }
        }

        if (!string.IsNullOrWhiteSpace(CsoComments))
        {
            Command.Comments = CsoComments;
        }
    }

    private async Task OnSubmit()
    {
        await form.ValidateAsync();

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
