using Cfo.Cats.Application.Common.Exceptions;
using Cfo.Cats.Application.Features.PRIs.Commands;
using Cfo.Cats.Application.Features.PRIs.DTOs;
using Cfo.Cats.Application.Features.PRIs.Queries;

namespace Cfo.Cats.Server.UI.Pages.Participants.Components;

public partial class CasePRI
{
    [Parameter] [EditorRequired] public string ParticipantId { get; set; } = null!;

    private PRIDto? _model;
    private AddPRI.PriReleaseDto? _priRelease;
    private AddPRI.PriMeetingDto? _preMeeting;
    private bool _notFound;

    protected override async Task OnInitializedAsync()
    {
        _model = null;
        try
        {
            var result = await GetNewMediator().Send(new GetParticipantPRI.Query
            {
                ParticipantId = ParticipantId
            });

            if (result is { Succeeded: true, Data: not null })
            {
                _model = result.Data;
                _priRelease = Mapper.Map<AddPRI.PriReleaseDto>(_model);
                _preMeeting = Mapper.Map<AddPRI.PriMeetingDto>(_model);
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
        }
    }
}