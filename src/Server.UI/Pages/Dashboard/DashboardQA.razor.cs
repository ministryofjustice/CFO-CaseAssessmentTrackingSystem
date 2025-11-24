using Cfo.Cats.Application.Common.Security;
using Cfo.Cats.Server.UI.Components.Identity;
using Cfo.Cats.Application.SecurityConstants;

namespace Cfo.Cats.Server.UI.Pages.Dashboard;

public partial class DashboardQA
{
    private MudDateRangePicker _picker = null!;

    private bool _visualMode = true;
    public string? SelectedUserId { get; set; }
    public string? SelectedDisplayName { get; set; }
    private DateRange _dateRange { get; set; } = new DateRange(new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1), DateTime.Today);
    
    [CascadingParameter]
    public UserProfile CurrentUser { get; set; } = null!;
    protected override void OnInitialized()
    {

    }
}