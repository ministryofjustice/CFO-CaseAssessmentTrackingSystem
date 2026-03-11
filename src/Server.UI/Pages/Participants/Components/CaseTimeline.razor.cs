using Cfo.Cats.Application.Common.Security;
using Cfo.Cats.Application.Features.Participants.DTOs;
using Cfo.Cats.Application.Features.Timelines.DTOs;
using Cfo.Cats.Application.Features.Timelines.PaginationQuery;
using Cfo.Cats.Application.SecurityConstants;
using Cfo.Cats.Domain.Common.Enums;
using Microsoft.EntityFrameworkCore;
using MudExtensions;
using Color = MudBlazor.Color;

namespace Cfo.Cats.Server.UI.Pages.Participants.Components;

public partial class CaseTimeline
{
    [CascadingParameter]
    public ParticipantSummaryDto ParticipantSummaryDto { get; set; } = default!;

    [CascadingParameter]
    public UserProfile UserProfile { get; set; } = default!;

    private PaginatedData<TimelineDto>? _model = null;

    private int pageNumber = 1;
    private int pageSize = 10;
    private readonly int[] pageSizeOptions = [10, 30, 100];

    //Hide CFO Users from providers on time line
    private bool hideUser = true;
    private HashSet<string> CFOTenantIds = new HashSet<string> { "1.", "1.1." };
    private string[] allowed = [RoleNames.QAOfficer, RoleNames.QASupportManager, RoleNames.QAManager, RoleNames.SMT, RoleNames.SystemSupport];

    protected override async Task OnInitializedAsync()
    {
        await LoadDataAsync();

        if (UserProfile.AssignedRoles.Any(r => allowed.Contains(r)))
        {
            hideUser = false;
        }
    }

    private async Task LoadDataAsync()
    {
        _model = await GetNewMediator().Send(new TimelinesWithPaginationQuery.Query()
        {
            ParticipantId = ParticipantSummaryDto.Id,
            CurrentUser = UserProfile,
            PageNumber = pageNumber,
            PageSize = pageSize
        });
    }

    private async Task OnPageChanged(int page)
    {
        pageNumber = page;
        await LoadDataAsync();
    }

    private async Task OnPageSizeChanged(int size)
    {
        pageSize = size;
        pageNumber = 1;
        await LoadDataAsync();
    }

    private string ItemRangeLabel =>
        _model is null ? string.Empty
        : $"{(_model.CurrentPage - 1) * pageSize + 1}-{Math.Min(_model.CurrentPage * pageSize, _model.TotalItems)} of {_model.TotalItems}";

    private Color GetColour(TimelineDto dto)
    {
        return dto.Title switch
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
}