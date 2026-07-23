using Cfo.Cats.Application.Common.Security;

namespace Cfo.Cats.Application.Features.Notifications.Specifications;

public class NotificationsAdvancedFilter : PaginationFilter
{
    public UserProfile? CurrentUser { get; set; }

    public bool ShowReadNotifications { get; set; } = false;

    public NotificationsListView ListView { get; set; } = NotificationsListView.Default;
    
    public NotificationType? Type { get; set; }
}

public enum NotificationsListView
{
    [Description("Default")] Default = 0,
}

public enum NotificationType
{
    [Description("All")] All = 0,
    [Description("Activities")] Activities = 1,
    [Description("Enrolments")] Enrolments = 2,
    [Description("Assigned")] Assigned = 3,
    [Description("Unassigned")] Unassigned = 4,
    [Description("Pri")] Pri = 5
}
