using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Ardalis.Specification;

namespace Cfo.Cats.Application.Features.Identity.Specifications;

public class IdentityAuditTrailAdvancedSpecification : Specification<IdentityAuditTrail>
{
    public IdentityAuditTrailAdvancedSpecification(IdentityAuditTrailAdvancedFilter filter)    
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
            .Where(p => p.ActionType == filter.IdentityActionType, filter.IdentityActionType is not null)
            .Where(
                p => p.UserName == filter.UserName,
                filter.UserName is not null
            )
            .Where(
                p => p.DateTime.Date == DateTime.Now.Date,
                filter!.ListView == IdentityAuditTrailListView.CreatedToday
            )
            .Where(p => p.DateTime >= last30day, filter.ListView == IdentityAuditTrailListView.Last30days);

    }
}