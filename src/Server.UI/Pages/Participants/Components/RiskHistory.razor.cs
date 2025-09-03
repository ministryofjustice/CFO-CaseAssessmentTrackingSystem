using Cfo.Cats.Application.Common.Exceptions;
using Cfo.Cats.Application.Features.Participants.DTOs;
using Cfo.Cats.Application.Features.Participants.Queries;
using Microsoft.AspNetCore.Http.HttpResults;

namespace Cfo.Cats.Server.UI.Pages.Risk.RiskComponents;
public partial class RiskHistory
{
    [Parameter, EditorRequired]
    public string ParticipantId { get; set; } = default!;

    private IEnumerable<RiskHistoryDto> _participantRisks = Enumerable.Empty<RiskHistoryDto>();
    private bool _notFound = false;
    private bool _isLoading = true;
    protected override async Task OnInitializedAsync()
    {
        try
        {
            var historyResult = await GetNewMediator().Send(new GetParticipantRiskHistory.Query()
            {
                ParticipantId = ParticipantId
            });

            if (historyResult.Succeeded && historyResult.Data is not null)
            {
                _participantRisks = historyResult.Data
                                        .OrderByDescending(r => r.CreatedDate);
            }
            else
            {
                _notFound = true;
            }
        }
        catch (NotFoundException)
        {
            _notFound = true;
        }
        finally
        {
            await base.OnInitializedAsync();
            _isLoading = false;
        }

    }
}