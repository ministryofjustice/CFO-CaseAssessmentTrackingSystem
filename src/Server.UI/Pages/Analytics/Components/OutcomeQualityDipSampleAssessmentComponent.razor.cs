using Cfo.Cats.Application.Features.Assessments.DTOs;
using Cfo.Cats.Application.Features.Assessments.Queries;
using Microsoft.JSInterop;
using System.Collections.Concurrent;

namespace Cfo.Cats.Server.UI.Pages.Analytics.Components;

public partial class OutcomeQualityDipSampleAssessmentComponent
{

    private IEnumerable<ParticipantAssessmentDto> _participantAssessments = Enumerable.Empty<ParticipantAssessmentDto>();
    private bool _isLoading = true;

    // Dictionaries to hold UI state, keyed by the Assessment's Guid
    private readonly ConcurrentDictionary<Guid, bool> _assessmentLoadingStates = new();
    private readonly ConcurrentDictionary<Guid, Application.Features.Assessments.DTOs.Assessment?> _assessmentsById = new();

    [Parameter]
    [EditorRequired]
    public string ParticipantId { get; set; } = default!;

    protected override async Task OnInitializedAsync()
    {
        try
        {

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
            _isLoading = false;
        }
    }

    private async Task LoadAssessmentById(bool isExpanded, Guid assessmentId)
    {
        try
        {
            if (isExpanded && !_assessmentsById.ContainsKey(assessmentId))
            {
                _assessmentLoadingStates[assessmentId] = true;

                var result = await GetNewMediator().Send(new GetAssessment.Query()
                {
                    ParticipantId = ParticipantId,
                    AssessmentId = assessmentId
                });

                if (result is { Succeeded: true, Data: not null })
                {
                    _assessmentsById[assessmentId] = result.Data;
                }
                
                
            }
        }
        finally
        {
            _assessmentLoadingStates.TryRemove(assessmentId, out _);
        }

    }
}