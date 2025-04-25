using Cfo.Cats.Application.Features.Participants.Commands;
using Cfo.Cats.Application.Features.Participants.DTOs;
using Cfo.Cats.Infrastructure.Constants;
using Cfo.Cats.Infrastructure.Services.Ordnance;
using FluentValidation;
using static Cfo.Cats.Application.Features.Participants.Commands.AddOrUpdateContactDetail;

namespace Cfo.Cats.Server.UI.Pages.Participants.Components;

public partial class AddressDialog(IAddressLookupService addressLookupService)
{
    private MudForm _form = new();
    private bool _saving = false;

    private string _errorAtLeastOneFieldProvided = string.Empty;
    [CascadingParameter] private IMudDialogInstance Dialog { get; set; } = default!;

    [Parameter]
    public required AddOrUpdateContactDetail.Command Model { get; set; }

    private async Task<IEnumerable<ParticipantAddressDto?>> Search(string searchText, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(searchText) is false)
        {
            var response = await addressLookupService.SearchAsync(searchText, CancellationToken.None);

            if (response is { Succeeded: true, Data: not null })
            {
                return response.Data;
            }

            Snackbar.Add(response.ErrorMessage, Severity.Error);
        }

        return [];
    }

    private async Task Submit()
    {
        try
        {
            //_errorAtLeastOneFieldProvided = string.Empty;
            _saving = true;

            await _form.Validate();


            var (isValid, objectLevelError) = await ValidateFormWithFluent(_form, Model, new A_BeValid());

            if (!isValid)
            {
                if (!string.IsNullOrWhiteSpace(objectLevelError))
                {
                    _errorAtLeastOneFieldProvided = objectLevelError;
                }

                return;
            }

            var result = await GetNewMediator().Send(Model);

            if (result.Succeeded)
            {
                Dialog.Close(DialogResult.Ok(true));
                Snackbar.Add(ConstantString.SaveSuccess, Severity.Info);
            }
            else
            {
                Snackbar.Add(result.ErrorMessage, Severity.Error);
            }

        }
        finally
        {
            _saving = false;
        }
    }
    public static async Task<(bool IsValid, string? ObjectLevelError)>
    ValidateFormWithFluent<TModel>(
        MudForm form,
        TModel model,
        IValidator<TModel> validator)
    {
        // Run MudBlazor validation (field-level)
        await form.Validate();

        // Run FluentValidation (model-level)
        var fluentResult = validator.Validate(model);

        // Get only object-level error (no PropertyName)
        var objectLevelError = fluentResult.Errors
            .FirstOrDefault(e => string.IsNullOrWhiteSpace(e.PropertyName))?.ErrorMessage;

        // Form is valid only if both checks pass
        //because once mandatory fields on the form are filled in the _form.IsValid becomes true even if the three fields are all empty
        var isValid = form.IsValid && objectLevelError == null;

        return (isValid, objectLevelError);
    }

}
