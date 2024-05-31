namespace Cfo.Cats.Application.Features.AuditTrails.Specifications;

public enum AuditTrailListView
{
    [Description("All")]
    All,

    [Description("My Change Histories")]
    My,

    [Description("Created Today")]
    CreatedToday,

    [Description("View of the last 30 days")]
    Last30days
}