using AutoMapper;
using Cfo.Cats.Application.Common.Security;
using Cfo.Cats.Application.Features.Participants.Commands;
using Cfo.Cats.Domain.Identity;
using FluentValidation;
using Microsoft.AspNetCore.Identity;

namespace Cfo.Cats.Server.UI.Pages.Participants.Components;
public partial class ReassignParticipantDialog
{

    private UserManager<ApplicationUser>? _userManager;
    private MudForm? _form;

    [EditorRequired]
    [Parameter]
    public ReassignParticipants.Command Model { get; set; } = null!;

    [CascadingParameter] private IMudDialogInstance MudDialog { get; set; } = default!;
    [Parameter] public UserProfile? UserProfile { get; set; }

    private bool _saving;

    private void Cancel() => MudDialog.Close();

    protected override void OnInitialized()
    {
        //TODO: replace this with new user lookup when that is merged into main.
        _userManager = ScopedServices.GetRequiredService<UserManager<ApplicationUser>>();
    }

    private async Task Submit()
    {
        try
        {
            _saving = true;

            await _form!.Validate();

            if (_form.IsValid)
            {
                var result = await GetNewMediator().Send(Model);
                if (result.Succeeded)
                {
                    MudDialog.Close(DialogResult.Ok(true));
                    Snackbar.Add("Participants reassigned", Severity.Info);
                }
                else
                {
                    Snackbar.Add(result.ErrorMessage, Severity.Error);
                }
            }
        }
        finally
        {
            _saving = false;
        }
    }

}