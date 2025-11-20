using Cfo.Cats.Application.Common.Interfaces.Locations;
using Cfo.Cats.Application.Features.Locations.DTOs;
using Cfo.Cats.Application.Features.Participants.DTOs;
using Cfo.Cats.Application.Features.Participants.Queries;
using Cfo.Cats.Application.Features.Participants.Specifications;
using Microsoft.AspNetCore.Components;

namespace Cfo.Cats.Server.UI.Pages.Participants;

public partial class Index 
{
    private LocationDto[] _locations = [];
    private ParticipantListView _listView = ParticipantListView.Default;
    private string _orderBy = "Id";
    private string _orderDirection = "ascending";
    private MudTable<ParticipantPaginationDto>? _table;
    private int _pageSize = 15;
    private int _pageNumber = 1;

    [Inject] public ILocationService LocationService { get; set; } = null!;
    
    protected override IRequest<Result<PaginatedData<ParticipantPaginationDto>>> CreateQuery() =>
        new ParticipantsWithPagination.Query()
        {
            CurrentUser = CurrentUser,
            JustMyCases = false,
            ListView = _listView,
            PageNumber = _pageNumber,
            PageSize = _pageSize,
            Locations = _locations.Select(x => x.Id).ToArray(),
            OrderBy = _orderBy,
            SortDirection = _orderDirection,
        };

    private async Task AddLocationFilter(LocationDto location)
    {
        if (_locations.Contains(location))
        {
            return;
        }

        _locations = [.._locations, location];
        await LoadDataAsync();
    }

    private async Task RemoveLocation(LocationDto location)
    {
        if (!_locations.Contains(location))
        {
            return;
        }

        _locations = _locations.Except([location]).ToArray();
        await LoadDataAsync();
    }

    private async Task OnChangedListView(ParticipantListView arg)
    {
        if (arg == _listView)
        {
            return;
        }
        _listView = arg;
        await LoadDataAsync();
    }

    private async Task OnSelectedPageChanged(int arg)
    {
        if (arg == _pageNumber)
        {
            return;
        }
        _pageNumber = arg;
        await LoadDataAsync();
    }

    private Task SortDirectionChanged(string field, SortDirection direction)
    {
        _orderBy = field;
        _orderDirection = direction == SortDirection.Descending ? "Descending" : "Ascending";
        return LoadDataAsync();
    }
}