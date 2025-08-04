using ApexCharts;
using Cfo.Cats.Application.Common.Security;
using Cfo.Cats.Application.Features.Dashboard.DTOs;
using Cfo.Cats.Application.Features.Dashboard.Queries;
using Cfo.Cats.Application.Features.Participants.Specifications;
using ChartType = ApexCharts.ChartType;

namespace Cfo.Cats.Server.UI.Pages.Dashboard.Components;

public partial class MyTeamsParticipantsComponent
{
    private bool _loading = true;
    private ParticipantCountSummaryDto? _participantSteps;
    private ChartData[]? _data;

    [CascadingParameter(Name="IsDarkMode")]
    public bool IsDarkMode { get; set; }

    private ApexChartOptions<ChartData> Options => new()
    {
        Chart = new Chart
        {
            Type = ChartType.Pie,
            Toolbar = new ApexCharts.Toolbar
            {
                Show = true,
                Export = new ExportOptions
                {
                    Csv = new ApexCharts.ExportCSV()
                    {
                        Filename = "Participants",
                    },
                    Png = new ExportPng()
                    {
                        Filename = "Particpants-Pie-Chart"
                    },
                    Svg = new ExportSvg()
                    {
                        Filename = "Particpants-Pie-Chart"
                    },
                }
            }
        },
        Theme = new Theme
        {
            Mode = IsDarkMode ? Mode.Dark : Mode.Light,
        }
    };

    [CascadingParameter] public UserProfile? UserProfile { get; set; }

    protected override async Task OnInitializedAsync()
    {
        try
        {
            var query = new GetMyTeamsParticipantsDashboard.Query()
            {
                CurrentUser = UserProfile!
            };

            var result = await GetNewMediator().Send(query);

            if (result is { Succeeded: true, Data: not null })
            {
                _participantSteps = result.Data;
                _data =
                [
                    new("Identified", result.Data.IdentifiedCases),
                    new("Enrolling", result.Data.EnrollingCases),
                    new("Submitted to PQA", result.Data.CasesAtPqa),
                    new("Submitted to Authority", result.Data.CasesAtCfo),
                    new("Approved", result.Data.ApprovedCases)
                ];
            }
        }
        finally
        {
            _loading = false;
        }
    }

    private string GetParticipantsUrl(ParticipantListView listView) =>
        $"/pages/participants?listView={listView.ToString()}";

    private void GotoParticipants() => Navigation.NavigateTo("/pages/participants");

    public record ChartData(string Description, int Count);
}