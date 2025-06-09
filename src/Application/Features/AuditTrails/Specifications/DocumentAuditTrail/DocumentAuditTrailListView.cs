namespace Cfo.Cats.Application.Features.AuditTrails.Specifications.DocumentAuditTrail;

public enum DocumentAuditTrailListView
{
    [Description("All")]
    All,

    [Description("My Events")]
    My,

    [Description("Created Today")]
    CreatedToday,

    [Description("View of the last 30 days")]
    Last30days
}