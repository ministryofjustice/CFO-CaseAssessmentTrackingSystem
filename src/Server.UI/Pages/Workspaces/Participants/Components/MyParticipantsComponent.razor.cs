using ApexCharts;
using Cfo.Cats.Application.Features.Dashboard.DTOs;
using Cfo.Cats.Application.Features.Dashboard.Queries;
using Cfo.Cats.Application.Features.Participants.Specifications;
using Cfo.Cats.Domain.Common.Contracts;
using Cfo.Cats.Domain.Common.Enums;
using MemoryPack.Formatters;
using Microsoft.JSInterop;

namespace Cfo.Cats.Server.UI.Pages.Workspaces.Participants.Components;

public partial class MyParticipantsComponent
{
    private ApexCharts.ApexChartOptions<DataItem>? _chartOptions;

    private DataItem[]? _dataItems = null;

    [CascadingParameter(Name="IsDarkMode")]
    public bool IsDarkMode { get; set; }

    protected override void OnInitialized() => _chartOptions = new()
    {
        Chart = new Chart
        {
            Type = ApexCharts.ChartType.Bar,
            Toolbar = new Toolbar
            {
                Show = true,
                Export = new ExportOptions
                {
                    Csv = new ExportCSV()
                    {
                        Filename = "Participants",
                    },
                    Png = new ExportPng()
                    {
                        Filename = "Particpants-Chart"
                    },
                    Svg = new ExportSvg()
                    {
                        Filename = "Particpants-Chart"
                    },
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

    private string GetParticipantsUrl(ParticipantListView listView) =>
        $"/pages/workspace/participants?listView={listView}";

    private void GotoParticipants() => Navigation.NavigateTo("/pages/workspace/participants");
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