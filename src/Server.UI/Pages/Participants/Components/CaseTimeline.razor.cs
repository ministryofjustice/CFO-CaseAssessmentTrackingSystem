using Cfo.Cats.Application.Common.Security;
using Cfo.Cats.Application.Features.Participants.DTOs;
using Cfo.Cats.Application.Features.Timelines.DTOs;
using Cfo.Cats.Application.Features.Timelines.PaginationQuery;
using Cfo.Cats.Application.Features.Timelines.Specifications;
using Cfo.Cats.Domain.Common.Enums;
using Color = MudBlazor.Color;

namespace Cfo.Cats.Server.UI.Pages.Participants.Components;

public partial class CaseTimeline
{
    [CascadingParameter]
    public ParticipantSummaryDto ParticipantSummaryDto { get; set; } = default!;

    [CascadingParameter]
    public UserProfile UserProfile { get; set; } = default!;

    private List<TimelineDto> _items = [];    

    //Hide CFO Users from providers on time line
    private bool _hideUser = true;
    private int _pageNumber = 1;
    private TimelineTrailListView _listView = TimelineTrailListView.All;
    private readonly HashSet<string> _cfoTenantIds = new HashSet<string> { "1.", "1.1." };

    protected override async Task OnInitializedAsync()
    {
        _hideUser = UserProfile.HasInternalRole() == false;
        await LoadDataAsync();
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

    private Task OnPaginationChanged(int page)
    {
        _pageNumber = page;
        return LoadDataAsync();
    }

    private Task OnListViewChanged(TimelineTrailListView listView)
    {
        _listView = listView;
        return LoadDataAsync();
    }

    protected override IRequest<Result<PaginatedData<TimelineDto>>> CreateQuery() 
        => new TimelinesWithPaginationQuery.Query()
            {
                ParticipantId = ParticipantSummaryDto.Id,
                CurrentUser = UserProfile,
                Keyword = null,
                ListView = _listView,
                OrderBy = "Created",
                SortDirection = SortDirection.Descending.ToString(),
                PageNumber = _pageNumber,
                PageSize = 5            
            };

}