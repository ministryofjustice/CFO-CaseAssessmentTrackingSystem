using Cfo.Cats.Application.Common.Exceptions;
using Cfo.Cats.Application.Features.Activities.DTOs;
using Cfo.Cats.Application.Features.Participants.DTOs;
using Cfo.Cats.Application.Features.Participants.Queries;

namespace Cfo.Cats.Server.UI.Pages.Risk.RiskComponents;
public partial class RiskHistory
{
    [Parameter, EditorRequired]
    public string ParticipantId { get; set; } = default!;

    private RiskHistoryDto[]? _participantRisks = null;
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
                                        .OrderByDescending(r => r.CreatedDate).ToArray();
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