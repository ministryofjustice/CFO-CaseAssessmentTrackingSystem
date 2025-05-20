using Cfo.Cats.Application.Features.Assessments.DTOs;
using Cfo.Cats.Application.Features.Assessments.Queries;
using Cfo.Cats.Application.Features.Participants.Queries;
using Microsoft.JSInterop;

namespace Cfo.Cats.Server.UI.Pages.Participants.Components;

public partial class CaseAssessment
{

    private Cfo.Cats.Application.Features.Assessments.DTOs.Assessment? _model;
    private IEnumerable<ParticipantAssessmentDto> _participantAssessments = Enumerable.Empty<ParticipantAssessmentDto>();
    private DateTime? _consentDate;

    private bool _notFoundAssessment = false;

    [Parameter]
    [EditorRequired]
    public string ParticipantId { get; set; } = default!;

    protected override async Task OnInitializedAsync()
    {
        if (_model is not null)
        {
            return;
        }

        try
        {
            var result = await GetNewMediator().Send(new GetAssessment.Query()
            {
                ParticipantId = ParticipantId
            });

            if (result.Succeeded)
            {
                _model = result.Data;
            }

            var query = new GetAssessmentScores.Query()
            {
                ParticipantId = ParticipantId
            };

            var resultpa = await GetNewMediator().Send(query);

            if (resultpa.Succeeded && resultpa.Data != null)
            {
                _participantAssessments = resultpa.Data
                                                .Where(pa => pa.Completed.HasValue)
                                                .OrderByDescending(pa => pa.CreatedDate);
                if (_participantAssessments?.Any() == true)
                {
                    var _participant = await GetNewMediator().Send(new GetParticipantById.Query()
                    {
                        Id = ParticipantId
                    });
                    _consentDate = _participant.CalculatedConsentDate;
                }
            }

        }
        finally
        {
            _notFoundAssessment = _model is null;
        }

    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        await JS.InvokeVoidAsync("removeInlineStyle", ".two-columns");
    }
}