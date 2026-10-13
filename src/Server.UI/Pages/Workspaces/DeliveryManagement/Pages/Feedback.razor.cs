using Cfo.Cats.Application.Common.Security;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace Cfo.Cats.Server.UI.Pages.Workspaces.DeliveryManagement.Pages;

public partial class Feedback
{
    private MudDateRangePicker _picker = null!;
    private bool _visualMode = true;
    private DateRange _dateRange = new(new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1), DateTime.Today);

    [CascadingParameter]
    public UserProfile CurrentUser { get; set; } = null!;

    private string TenantId => CurrentUser.TenantId
        ?? throw new InvalidOperationException("Current user TenantId is required.");

    private string FeedbackKey => TenantId + (_dateRange.Start?.Ticks ?? 0) + (_dateRange.End?.Ticks ?? 0);
}