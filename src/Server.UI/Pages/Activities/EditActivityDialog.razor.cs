using Cfo.Cats.Application.Features.Activities.Commands;
using Cfo.Cats.Application.Features.Activities.Queries;
using Cfo.Cats.Infrastructure.Constants;

namespace Cfo.Cats.Server.UI.Pages.Activities;

public partial class EditActivityDialog
{
    private AddActivity.Command? _model;
    private MudForm _form = new();
    private bool _saving;

    [CascadingParameter] public required IMudDialogInstance Dialog { get; set; }

    [Parameter, EditorRequired] public required Guid ActivityId { get; set; }

    [Parameter] public string ParticipantId { get; set; } = string.Empty;

    [Parameter] public string ParticipantFirstName { get; set; } = string.Empty;

    [Parameter] public string ParticipantLastName { get; set; } = string.Empty;

    protected override async Task OnInitializedAsync()
    {
        var activity = await GetNewMediator().Send(new GetActivityById.Query()
        {
            Id = ActivityId
        });

        _model = Mapper.Map<AddActivity.Command>(activity);
        _model.ActivityId = ActivityId;

        await base.OnInitializedAsync();
    }

    private async Task Submit()
    {
        if (_model is null)
        {
            return;
        }

        try
        {
            _saving = true;

            await _form.Validate();

            if (_form.IsValid is false)
            {
                return;
            }

            var result = await GetNewMediator().Send(_model);

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
}