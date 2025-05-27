using Cfo.Cats.Application.Features.Assessments.DTOs;
using Cfo.Cats.Application.Features.Assessments.Queries;
using Microsoft.JSInterop;

namespace Cfo.Cats.Server.UI.Pages.Participants.Components;

public partial class CaseAssessment
{
    private Application.Features.Assessments.DTOs.Assessment? _model;
    private IEnumerable<ParticipantAssessmentDto> _participantAssessments = Enumerable.Empty<ParticipantAssessmentDto>();  

    private bool _loading = true;

    [Parameter]
    [EditorRequired]
    public string ParticipantId { get; set; } = default!;

    [Parameter]
    [EditorRequired]
    public DateOnly? ConsentDate { get; set; }

    protected override async Task OnInitializedAsync()
    {
        try
        {
            var result = await GetNewMediator().Send(new GetAssessment.Query()
            {
                ParticipantId = ParticipantId
            });
           
            if (result is { Succeeded: true, Data: not null })
            {
                _model = result.Data;
            }

            var query = new GetAssessmentScores.Query()
            {
                ParticipantId = ParticipantId
            };

            var assessmentHistoryResult = await GetNewMediator().Send(query);

            if (assessmentHistoryResult is { Succeeded: true, Data: not null })
            {
                _participantAssessments = assessmentHistoryResult.Data
                                                .Where(pa => pa.Completed.HasValue)
                                                .OrderByDescending(pa => pa.CreatedDate);
            }
        }
        finally
        {
            _loading = false;
        }
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        await JS.InvokeVoidAsync("removeInlineStyle", ".two-columns");
    }
}