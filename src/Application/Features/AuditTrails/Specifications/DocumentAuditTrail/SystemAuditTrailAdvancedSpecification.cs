namespace Cfo.Cats.Application.Features.AuditTrails.Specifications.DocumentAuditTrail;

public class DocumentAuditTrailAdvancedSpecification : Specification<Domain.Entities.Documents.DocumentAuditTrail>
{
    public DocumentAuditTrailAdvancedSpecification(DocumentAuditTrailAdvancedFilter filter)
    {
        var today = DateTime.Now.ToUniversalTime().Date;
        var start = Convert.ToDateTime(
            today.ToString("yyyy-MM-dd", CultureInfo.CurrentCulture) + " 00:00:00",
            CultureInfo.CurrentCulture
        );
        var end = Convert.ToDateTime(
            today.ToString("yyyy-MM-dd", CultureInfo.CurrentCulture) + " 23:59:59",
            CultureInfo.CurrentCulture
        );
        var last30day = Convert.ToDateTime(
            today.AddDays(-30).ToString("yyyy-MM-dd", CultureInfo.CurrentCulture) + " 00:00:00",
            CultureInfo.CurrentCulture
        );

        Query
            .Where(q => q.RequestType == filter.RequestType, filter.RequestType is not null)
            .Where(
                p => p.UserId == filter.CurrentUser!.UserId,
                filter is { ListView: DocumentAuditTrailListView.My, CurrentUser: not null }
            )
            .Where(
                p => p.OccurredOn.Date == DateTime.Now.Date,
                filter.ListView == DocumentAuditTrailListView.CreatedToday
            )
            .Where(p => p.OccurredOn >= last30day, filter.ListView == DocumentAuditTrailListView.Last30days)
            .Where(
                p => p.Document!.Title!.Contains(filter.Keyword!) || p.Document!.Description!.Contains(filter.Keyword!), 
                filter.Keyword is not null);
    }
}
