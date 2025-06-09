using Cfo.Cats.Application.Common.Security;
using Cfo.Cats.Domain.Entities.Documents;

namespace Cfo.Cats.Application.Features.AuditTrails.Specifications.DocumentAuditTrail;

public class DocumentAuditTrailAdvancedFilter : PaginationFilter
{
    public DocumentAuditTrailRequestType? RequestType { get; set; }
    public UserProfile? CurrentUser { get; set; }
    public DocumentAuditTrailListView ListView { get; set; } = DocumentAuditTrailListView.All;
}
