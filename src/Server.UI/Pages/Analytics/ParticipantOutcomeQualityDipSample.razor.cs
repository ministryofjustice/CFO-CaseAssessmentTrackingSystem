
using Cfo.Cats.Application.Common.Interfaces.Identity;
using Cfo.Cats.Application.Common.Security;
using Cfo.Cats.Application.Features.ManagementInformation.Commands.AddOutcomeQualityDipSampleCso;
using Cfo.Cats.Application.Features.PerformanceManagement.DTOs;
using Cfo.Cats.Application.Features.PerformanceManagement.Queries;
using Cfo.Cats.Application.Features.Timelines.DTOs;
using Cfo.Cats.Domain.Common.Enums;
using Microsoft.JSInterop;

namespace Cfo.Cats.Server.UI.Pages.Analytics;

public partial class ParticipantOutcomeQualityDipSample
{
    [Parameter]
    public required Guid SampleId { get; set; }

    [Parameter]
    public required string ParticipantId { get; set; }

    [CascadingParameter]
    public UserProfile UserProfile { get; set; } = null!;

    [Inject]
    public IUserService UserService { get; set; } = null!;


    private Command? _command;

    private Command SubmitCommand
    {
        get
        {
            return _command ??= new Command
            {
                CurrentUser = UserProfile,
                ParticipantId = ParticipantId,
                DipSampleId = SampleId
            };
        }
    }

    private bool _isLoading = true;
    
    private ParticipantDipSampleDto? _participant;

    private string? _error;
    
    protected override async Task OnInitializedAsync()
    {

        try
        {
            var query = new GetOutcomeQualityDipSampleParticipant.Query()
            {
                ParticipantId = ParticipantId
            };

            var mediator = GetNewMediator();

            var dipSampleDtoResult = await mediator.Send(query, ComponentCancellationToken);

            if (IsDisposed == false)
            {
                if (dipSampleDtoResult is { Succeeded: true, Data: not null })
                {
                    _participant = dipSampleDtoResult.Data;
                }
                else
                {
                    _error = dipSampleDtoResult.ErrorMessage;
                }
            }

        }
        finally
        {
            _isLoading = false;
        }
        
    }
    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        await JSRuntime.InvokeVoidAsync("removeInlineStyle", ".two-columns");
    }

    private async Task CsoResponseSubmitted(Command command)
    {
        var mediator = GetNewMediator();
        var result = await mediator.Send(command);
        if (IsDisposed == false)
        {
            if (result is { Succeeded: true })
            {
                Snackbar.Add("Submission saved", Severity.Info);
                Navigation.NavigateTo($"/pages/analytics/outcome-quality-dip-sampling/{SampleId}/");
            }
            else
            {
                Snackbar.Add($"Failed: {result.ErrorMessage}", Severity.Error);
            }
        }
    }
}
