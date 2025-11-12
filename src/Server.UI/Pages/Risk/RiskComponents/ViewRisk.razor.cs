using Cfo.Cats.Application.Common.Exceptions;
using Cfo.Cats.Application.Features.Participants.DTOs;
using Cfo.Cats.Application.Features.Participants.Queries;

namespace Cfo.Cats.Server.UI.Pages.Risk.RiskComponents;
public partial class ViewRisk
{
    [Parameter]
    [EditorRequired]
    public string ParticipantId { get; set; } = default!;
    [Parameter]
    public Guid? RiskId { get; set; }

    private MudForm? form;
    private RiskDto? _model;
    private bool _notFound = false;
    private bool _isLoading = true;
    protected override async Task OnInitializedAsync()
    {
        try
        {
            var result = await GetNewMediator().Send(new GetParticipantRisk.Query()
            {
                ParticipantId = ParticipantId,
                RiskId = RiskId,
                ReadOnly = true
            });

            if (result is { Succeeded: true, Data: not null })
            {
                _model = result.Data;
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