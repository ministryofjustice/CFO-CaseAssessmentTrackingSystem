namespace Cfo.Cats.Application.Features.Identity.Specifications;

public enum IdentityAuditTrailListView
{
    [Description("All")]
    All,

    [Description("Created Today")]
    CreatedToday,

    [Description("View of the last 30 days")]
    Last30days
}