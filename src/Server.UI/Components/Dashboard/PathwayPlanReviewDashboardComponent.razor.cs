using ApexCharts;
using Cfo.Cats.Application.Features.Dashboard.Commands;
using Cfo.Cats.Application.Features.Dashboard.Queries;
using Cfo.Cats.Infrastructure.Constants;

namespace Cfo.Cats.Server.UI.Components.Dashboard;

public partial class PathwayPlanReviewDashboardComponent
{
    [Parameter]
    public string UserId { get; set; } = null!;
    [Parameter]
    public string TenantId { get; set; } = null!;

    [EditorRequired, Parameter]
    public bool VisualMode { get; set; }

    [CascadingParameter(Name = "IsDarkMode")]
    public bool IsDarkMode { get; set; }

    private bool ShowOverdueOnly { get; set; } = false;
    private bool _downloading;

    private GetPathwayPlans.Query Query { get; set; } = default!;

    protected override IQuery<Result<GetPathwayPlans.PathwayPlanDto>> CreateQuery()
     => new GetPathwayPlans.Query()
     {
         CurrentUser = CurrentUser,
         UserId = UserId,
         TenantId = TenantId
     };

    private ApexCharts.ApexChartOptions<GetPathwayPlans.LocationDetail> Options => new()
    {
        Chart = new ApexCharts.Chart
        {
            Stacked = true
        },
        PlotOptions = new ApexCharts.PlotOptions
        {
            Bar = new ApexCharts.PlotOptionsBar
            {
                Horizontal=false,
                DataLabels = new ApexCharts.PlotOptionsBarDataLabels
                {
                    Total = new ApexCharts.BarTotalDataLabels
                    {
                        Enabled = true,
                        Style = new ApexCharts.BarDataLabelsStyle
                        {
                            FontWeight = "800",
                            Color = IsDarkMode ? "#FFFFFF" : "#000000",
                        }
                    }
                },
            },
        },
        Yaxis = new List<YAxis>
        {
            new YAxis
            {
                Min = 0,
                ForceNiceScale = true
            }
        },
        Theme = new ApexCharts.Theme
        {
            Mode = IsDarkMode ? ApexCharts.Mode.Dark : ApexCharts.Mode.Light
        },
        Colors = new List<string> { "#5cb85c", "#d9534f" }
    };

    private IEnumerable<GetPathwayPlans.PathwayPlanReviewTabularData> FilteredTabularData =>
        ShowOverdueOnly && Data is not null 
            ? Data.TabularData.Where(x => x.IsOverdue) 
            : Data?.TabularData ?? Enumerable.Empty<GetPathwayPlans.PathwayPlanReviewTabularData>();

    private GetPathwayPlans.LocationDetail[] FilteredChartData =>
        ShowOverdueOnly && Data is not null
            ? Data.ChartData.Select(x => new GetPathwayPlans.LocationDetail(
                x.LocationName, 
                x.LocationType, 
                x.OverdueCount, 
                x.OverdueCount))
              .Where(x => x.TotalCount > 0)
              .ToArray()
            : Data?.ChartData ?? Array.Empty<GetPathwayPlans.LocationDetail>();

    private int CustodyCount =>
        ShowOverdueOnly && Data is not null
            ? Data.ChartData.Where(d => d.LocationType?.IsCustody == true).Sum(d => d.OverdueCount)
            : Data?.Custody ?? 0;

    private int CommunityCount =>
        ShowOverdueOnly && Data is not null
            ? Data.ChartData.Where(d => d.LocationType?.IsCommunity == true).Sum(d => d.OverdueCount)
            : Data?.Community ?? 0;

    private async Task ShowReviewNotes(string notes)
    {
        var parameters = new DialogParameters
        {
            { nameof(ReviewNotesDialog.Notes), notes }
        };
        
        var options = new DialogOptions 
        { 
            CloseButton = true,
            MaxWidth = MaxWidth.Small,
            FullWidth = true
        };
        
        await DialogService.ShowAsync<ReviewNotesDialog>("Review Notes", parameters, options);
    }
    
    private async Task OnExport()
    {
        try
        {
            _downloading = true;

            var result = await Service.Send(new ExportPathwayPlanReviewDashboard.Command
            {
                Request = new ExportPathwayPlanReviewDashboard.PathwayPlanReviewDashboardExportRequest
                {
                    UserId = UserId,
                    TenantId = TenantId,
                    ShowOverdueOnly = ShowOverdueOnly
                }
            });

            if (result.Succeeded)
            {
                Snackbar.Add(ConstantString.ExportSuccess, Severity.Info);
            }
            else
            {
                Snackbar.Add(result.ErrorMessage, Severity.Error);
            }
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "An error has occurred while generating the pathway plan review dashboard export");
            Snackbar.Add("An error has occurred while generating the pathway plan review dashboard export.", Severity.Error);
        }
        finally
        {
            _downloading = false;
        }
    }

}