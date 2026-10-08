using ApexCharts;
using Cfo.Cats.Application.Features.Dashboard.DTOs;
using Cfo.Cats.Application.Features.Dashboard.Queries;
using Cfo.Cats.Application.Features.Participants.Queries;
using Cfo.Cats.Application.Features.Participants.Specifications;
using Cfo.Cats.Domain.Common.Enums;
using Cfo.Cats.Server.UI.Pages.Workspaces.Participants.Services;
using Cfo.Cats.Server.UI.Services;

namespace Cfo.Cats.Server.UI.Pages.Workspaces.Participants.Components;

public partial class MyParticipantsComponent
{
    private ApexChartOptions<DataItem>? _chartOptions;

    private DataItem[]? _dataItems;

    [CascadingParameter(Name="IsDarkMode")]
    public bool IsDarkMode { get; set; }

    [Inject]
    public CatsSessionStorage SessionStorage { get;set; } = null!;

    protected override void OnInitialized() => _chartOptions = new()
    {
        Chart = new Chart
        {
            Type = ApexCharts.ChartType.Bar,
            Toolbar = new Toolbar
            {
                Show = true,
                Tools = new Tools
                {
                    Download = true,
                    Selection = false,
                    Zoom = false,
                    Zoomin = false,
                    Zoomout = false,
                    Pan = false,
                    Reset = false
                },
                Export = new ExportOptions
                {
                    Csv = new ExportCSV { Filename = "Participants-Chart" },
                    Png = new ExportPng { Filename = "Participants-Chart" },
                    Svg = new ExportSvg { Filename = "Participants-Chart" }
                }
            }
        },
        Theme = new Theme
        {
            Mode = IsDarkMode ? Mode.Dark : Mode.Light,
        },
        DataLabels = new DataLabels()
        {
            Enabled = true
        },
        PlotOptions = new PlotOptions
        {
            Bar = new PlotOptionsBar
            {
                Horizontal = true
            }
        },
        Tooltip = new()
        {
            Enabled = false
        }
    };

    private string PointColour(DataItem item) => item.Colour;

    private DataItem? _selectedItem;

    private Task OnDataPointSelected(SelectedData<DataItem> selection)
    {
        _selectedItem = selection.DataPoint?.Items?.FirstOrDefault();

        if (_selectedItem is not null)
        {
            SessionStorage.SetAsync(
                ParticipantsSessionData.FromQuery(
                   new ParticipantsWithPagination.Query()
                    {
                        OwnerId = CurrentUser.AssignedRoles.Length == 0 ? CurrentUser.UserId : null,
                        ListView =  _selectedItem.Key switch{
                            "Enrolling" => ParticipantListView.Enrolling,
                            "Approved" => ParticipantListView.Approved,
                            "Identified" => ParticipantListView.Identified,
                            "Submitted to PQA" => ParticipantListView.SubmittedToProvider,
                            "Submitted to Authority" => ParticipantListView.SubmittedToQa,
                             _ => ParticipantListView.Default
                        },
                        PageNumber = 1,
                        PageSize = 10,
                        Keyword = null,
                        OrderBy = "Id",
                        SortDirection = "Ascending"
                    },
                    false
            ));
            Navigation.NavigateTo(ParticipantLinks.All.Href, false);

        }

        return Task.CompletedTask;
    }
    
    protected override IQuery<Result<ParticipantCountSummaryDto>> CreateQuery() => 
       new GetMyParticipantsDashboard.Query()
       {
           CurrentUser = CurrentUser,
           IncludeTeams = CurrentUser.AssignedRoles.Length > 0
       };

    protected override void OnDataLoaded(ParticipantCountSummaryDto data)
    {
        _dataItems = [
            new ("Identified", data.IdentifiedCases, EnrolmentStatus.IdentifiedStatus.Colour, 0),
            new ("Enrolling", data.EnrollingCases, EnrolmentStatus.EnrollingStatus.Colour, 1),
            new ("Submitted to PQA", data.CasesAtPqa, EnrolmentStatus.SubmittedToProviderStatus.Colour, 2),
            new("Submitted to Authority", data.CasesAtCfo, EnrolmentStatus.SubmittedToAuthorityStatus.Colour, 3),
            new("Approved", data.ApprovedCases, EnrolmentStatus.ApprovedStatus.Colour, 4)
        ];
        
        base.OnDataLoaded(data);
    }

    protected class DataItem(string key, int count, string colour, int order)
    {
        public string Key { get; } = key;
        public int Count { get; } = count;
        public string Colour { get;} = colour;

        public int Order { get; } = order;
    }
}
