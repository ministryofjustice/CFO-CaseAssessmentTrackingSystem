namespace Cfo.Cats.Server.UI.Pages.Workspaces.Provider.Pages;

public partial class Performance
{
    private MudDateRangePicker _picker = null!;

    private DateRange _dateRange = new(new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1), DateTime.Today);

    private string PerformanceKey => $"{SelectionKey}|{_dateRange.Start?.Ticks ?? 0}|{_dateRange.End?.Ticks ?? 0}";
}
