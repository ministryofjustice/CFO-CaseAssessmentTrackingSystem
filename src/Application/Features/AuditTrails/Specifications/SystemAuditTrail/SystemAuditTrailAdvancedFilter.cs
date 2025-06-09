using Cfo.Cats.Application.Common.Security;

namespace Cfo.Cats.Application.Features.AuditTrails.Specifications.SystemAuditTrail;

public class SystemAuditTrailAdvancedFilter : PaginationFilter
{
    public Dictionary<string, object>? PrimaryKey { get; set; }

    public AuditType? AuditType { get; set; }
    public SystemAuditTrailListView ListView { get; set; } = SystemAuditTrailListView.All;
    public UserProfile? CurrentUser { get; set; }
}
