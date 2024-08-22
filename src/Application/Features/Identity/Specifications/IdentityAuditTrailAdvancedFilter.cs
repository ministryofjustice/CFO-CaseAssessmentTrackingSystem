namespace Cfo.Cats.Application.Features.Identity.Specifications;

public class IdentityAuditTrailAdvancedFilter : PaginationFilter
{
    public IdentityActionType? IdentityActionType { get; set; } 
    public IdentityAuditTrailListView ListView { get; set; } = IdentityAuditTrailListView.All;
    public string UserName { get; set;} = default!;
}
