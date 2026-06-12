using AutoMapper;
using Cfo.Cats.Application.Common.Security;
using Cfo.Cats.Application.Features.Identity.DTOs;
using Cfo.Cats.Application.Features.Participants.Commands;
using Cfo.Cats.Domain.Identity;
using FluentValidation;
using Microsoft.AspNetCore.Identity;

namespace Cfo.Cats.Server.UI.Pages.Participants.Components;
public partial class ReassignParticipantDialog
{
    private MudForm? _form;

    [EditorRequired]
    [Parameter]
    public ReassignParticipants.Command Model { get; set; } = null!;

    [CascadingParameter] private IMudDialogInstance MudDialog { get; set; } = default!;
    [Parameter] public UserProfile? UserProfile { get; set; }

    private bool _saving;

    private void Cancel() => MudDialog.Close();

    private async Task Submit()
    {
        try
        {
            _saving = true;

            await _form!.ValidateAsync();

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

    private void OnUserSelectedChanged(ApplicationUserDto user) => Model.AssigneeId = user.Id;

}