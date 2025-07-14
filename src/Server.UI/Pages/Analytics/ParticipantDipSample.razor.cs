
using Cfo.Cats.Application.Common.Interfaces.Identity;
using Cfo.Cats.Application.Common.Security;
using Cfo.Cats.Application.Features.PerformanceManagement.DTOs;
using Cfo.Cats.Application.Features.PerformanceManagement.Queries;
using Cfo.Cats.Application.Features.Timelines.DTOs;
using Cfo.Cats.Domain.Common.Enums;

namespace Cfo.Cats.Server.UI.Pages.Analytics;

public partial class ParticipantDipSample
{
    [Parameter]
    public required Guid SampleId { get; set; }

    [Parameter]
    public required string ParticipantId { get; set; }

    [CascadingParameter]
    public UserProfile UserProfile { get; set; } = null!;
    
    [Inject]
    private ILogger<ParticipantDipSample> Logger { get; set; } = null!;

    [Inject]
    public IUserService UserService { get; set; } = null!;

    private bool _isLoading = true;
    
    private ParticipantDipSampleDto? _participant;
    
    private string? _error;

    protected override async Task OnInitializedAsync()
    {

        try
        {
            var query = new GetDipSampleParticipant.Query()
            {
                ParticipantId = ParticipantId
            };

            var result = await GetNewMediator().Send(query, ComponentCancellationToken);

            if (IsDisposed == false)
            {
                if (result is { Succeeded: true, Data: not null })
                {
                    _participant = result.Data;
                }
                else
                {
                    _error = result.ErrorMessage;
                }
            }

        }
        finally
        {
            _isLoading = false;
        }
        
    }

    private Color GetColour(TimelineDto dto) => dto.Title switch
    {
        nameof(TimelineEventType.Participant) => Color.Primary,
        nameof(TimelineEventType.Enrolment) => Color.Success,
        nameof(TimelineEventType.Consent) => Color.Secondary,
        nameof(TimelineEventType.Assessment) => Color.Info,
        nameof(TimelineEventType.PathwayPlan) => Color.Warning,
        nameof(TimelineEventType.Bio) => Color.Error,
        nameof(TimelineEventType.Dms) => Color.Dark,
        _ => Color.Primary
    };

    
}
