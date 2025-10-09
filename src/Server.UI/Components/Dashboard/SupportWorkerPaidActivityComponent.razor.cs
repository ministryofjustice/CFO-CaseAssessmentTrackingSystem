using ApexCharts;
using Cfo.Cats.Application.Features.Dashboard.Queries;
using ChartType = ApexCharts.ChartType;

namespace Cfo.Cats.Server.UI.Components.Dashboard;

public partial class SupportWorkerPaidActivityComponent
{
    [EditorRequired, Parameter]
    public DateRange? DateRange { get; set; }

    [EditorRequired, Parameter]
    public string UserId { get; set; } = null!;

    [EditorRequired, Parameter]
    public bool VisualMode { get; set; }

    [CascadingParameter(Name = "IsDarkMode")]
    public bool IsDarkMode { get; set; }

    private string[] xLabels = Array.Empty<string>();
    private List<ChartSeries> barSeries = new();

    protected override IRequest<Result<GetPaidActivitiesPerSupportWorker.PaidActivitiesPerSupportWorkerDto>> CreateQuery()
     => new GetPaidActivitiesPerSupportWorker.Query()
     {
         CurrentUser = CurrentUser,
         UserId = UserId,
         StartDate = DateRange?.Start ?? throw new InvalidOperationException("DateRange not set"),
         EndDate = DateRange?.End ?? throw new InvalidOperationException("DateRange not set")
     };

    protected override async Task OnParametersSetAsync()
    {
        await base.OnParametersSetAsync();
        if (Data is null)
        {
            return;
        }
        // 1. Distinct locations (become Y-axis in horizontal bar)
        var locations = Data.Details
            .Select(d => d.Name)
            .Distinct()
            .OrderBy(name => name)
            .ToArray();

        xLabels = locations;

        // 2. Distinct activity types (each becomes a series)
        var activityTypes = Data.Details
            .Select(d => d.ActivityType)
            .Distinct()
            .OrderBy(type => type)
            .ToList();

        // 3. Generate a series per activity type
        barSeries = activityTypes.Select(activityType =>
        {
            double[] values = locations.Select(location =>
                (double)Data.Details
                    .Where(d => d.Name == location && d.ActivityType == activityType)
                    .Sum(d => d.TotalCount)
            ).ToArray();

            return new ChartSeries
            {
                Name = activityType,
                Data = values
            };
        }).ToList();
    }

}
