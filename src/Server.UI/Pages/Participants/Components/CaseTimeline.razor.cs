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

    private List<TimelineDto> _items = [];
    private int _totalItems;
    private int _pageNumber = 1;
    private bool _loadingMore;
    private const int PageSize = 50;

    //Hide CFO Users from providers on time line
    private bool _hideUser = true;
    private readonly HashSet<string> _cfoTenantIds = new HashSet<string> { "1.", "1.1." };
    private readonly string[] _allowedRoles = [RoleNames.QAOfficer, RoleNames.QASupportManager, RoleNames.QAManager, RoleNames.SMT, RoleNames.SystemSupport];

    protected override async Task OnInitializedAsync()
    {
        await LoadPageAsync();

        if (UserProfile.AssignedRoles.Any(r => _allowedRoles.Contains(r)))
        {
            _hideUser = false;
        }
    }

    private async Task LoadPageAsync()
    {
        var result = await GetNewMediator().Send(BuildQuery());

        _items.AddRange(result.Items);
        _totalItems = result.TotalItems;
    }
    private TimelinesWithPaginationQuery.Query BuildQuery() =>
    new()
    {
        ParticipantId = ParticipantSummaryDto.Id,
        CurrentUser = UserProfile,
        PageNumber = _pageNumber,
        PageSize = PageSize
    };
    private async Task ShowMoreAsync()
    {
        try
        {
            _loadingMore = true;
            _pageNumber++;
            await LoadPageAsync();
        }
        finally
        {
            _loadingMore = false;
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