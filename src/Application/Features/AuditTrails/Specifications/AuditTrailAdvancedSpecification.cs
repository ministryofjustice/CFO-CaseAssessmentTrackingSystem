﻿namespace Cfo.Cats.Application.Features.AuditTrails.Specifications;

public class AuditTrailAdvancedSpecification : Specification<AuditTrail>
{
    public AuditTrailAdvancedSpecification(AuditTrailAdvancedFilter filter)
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
            .Where(p => p.PrimaryKey == filter.PrimaryKey, filter.PrimaryKey is not null)
            .Where(p => p.AuditType == filter.AuditType, filter.AuditType is not null)
            .Where(
                p => p.UserId == filter.CurrentUser!.UserId,
                filter is { ListView: AuditTrailListView.My, CurrentUser: not null }
            )
            .Where(
                p => p.DateTime.Date == DateTime.Now.Date,
                filter.ListView == AuditTrailListView.CreatedToday
            )
            .Where(p => p.DateTime >= last30day, filter.ListView == AuditTrailListView.Last30days)
            .Where(
                x => x.TableName!.Contains(filter.Keyword!) || x.Owner!.DisplayName!.Contains(filter.Keyword!),
                !string.IsNullOrEmpty(filter.Keyword)
            );
    }
}
