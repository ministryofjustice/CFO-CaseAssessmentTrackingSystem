using Cfo.Cats.Application.Common.Interfaces.Identity;
using Cfo.Cats.Application.Common.Security;
using Cfo.Cats.Application.Features.ManagementInformation.Commands;
using Cfo.Cats.Application.Features.ManagementInformation.Commands.AddOutcomeQualityDipSampleCso;
using Cfo.Cats.Application.Features.PerformanceManagement.DTOs;
using Cfo.Cats.Application.Features.PerformanceManagement.Queries;
using Microsoft.JSInterop;

namespace Cfo.Cats.Server.UI.Pages.Analytics.OutcomeQualityDipSample;

public partial class ParticipantDetails
{
    [Parameter]
    public required Guid SampleId { get; set; }

    [Parameter]
    public required string ParticipantId { get; set; }

    [CascadingParameter]
    public UserProfile UserProfile { get; set; } = null!;

    [Inject]
    public IUserService UserService { get; set; } = null!;

    private List<BreadcrumbItem> Items =>
    [
        new("Outcome Quality", href: "/pages/analytics/outcome-quality-dip-sampling/", icon: Icons.Material.Filled.Home),
        new("Sample", href: $"/pages/analytics/outcome-quality-dip-sampling/{SampleId}", icon: Icons.Material.Filled.List),
        new(ParticipantId, href: $"/pages/analytics/outcome-quality-dip-sampling/{SampleId}/{ParticipantId}", icon: Icons.Material.Filled.Person)
    ];

    private Command? _csoCommand;

    private Command SubmitCsoCommand
    {
        get
        {
            return _csoCommand ??= new()
            {
                CurrentUser = UserProfile,
                ParticipantId = ParticipantId,
                DipSampleId = SampleId
            };
        }
        set => _csoCommand = value;
    }

    private AddOutcomeQualityDipSampleCpm.Command? _cpmCommand;

    private AddOutcomeQualityDipSampleCpm.Command SubmitCpmCommand
    {
        get
        {
            return _cpmCommand ??= new()
            {
                CurrentUser = UserProfile,
                ParticipantId = ParticipantId,
                DipSampleId = SampleId
            };
        }
        set
        {
            _cpmCommand = value;
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
                ParticipantId = ParticipantId,
                SampleId = SampleId
            };

            var mediator = GetNewMediator();

            var dipSampleDtoResult = await mediator.Send(query, ComponentCancellationToken);

            if (IsDisposed == false)
            {
                if (dipSampleDtoResult is { Succeeded: true, Data: not null })
                {
                    _participant = dipSampleDtoResult.Data;

                    // Saturate answers

                    SubmitCpmCommand = SubmitCpmCommand with
                    {
                        Comments = _participant.CpmComments,
                        ComplianceAnswer = _participant.CpmAnswer
                    };

                    SubmitCsoCommand = SubmitCsoCommand with
                    {
                        Comments = _participant.CsoComments,
                        ComplianceAnswer = _participant.CsoAnswer,
                        HasClearParticipantJourney = _participant.HasClearParticipantJourney,
                        ShowsTaskProgression = _participant.ShowsTaskProgression,
                        ActivitiesLinkToTasks = _participant.ActivitiesLinkToTasks,
                        TTGDemonstratesGoodPRIProcess = _participant.TTGDemonstratesGoodPRIProcess,
                        SupportsJourneyAndAlignsWithDoS = _participant.SupportsJourneyAndAlignsWithDoS,
                    };
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
        => await JSRuntime.InvokeVoidAsync("removeInlineStyle", ".two-columns");

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

    private async Task CpmResponseSubmitted(AddOutcomeQualityDipSampleCpm.Command command)
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
